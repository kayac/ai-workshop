using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// キャラクター
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class GameCharacter : GameCarriedObject
{

	/// <summary>
	/// 移動速度(毎秒あたり)
	/// </summary>
	[Header("移動速度(毎秒あたり)")]
	[SerializeField]
	private float _speed;

	private Rigidbody2D _rigidbody2D;

	private CharacterLevelData _levelData;

	private CharacterLevelData _nextLevelData;


	/// <summary>
	/// 現在のレベル
	/// </summary>
	/// <returns></returns>
	public int level { get { return _levelData.level; }} 

	/// <summary>
	/// 現在の経験値
	/// </summary>
	/// <returns></returns>
	public int exp { get; private set; }

	/// <summary>
	/// 味方/敵の判別
	/// </summary>
	/// <returns></returns>
	public Const.Side side { get; private set; }
	
	/// <summary>
	/// 産卵するまでのカウント
	/// 何かを食べると増える 
	/// </summary>
	/// <returns></returns>
	public int currentLayEggCount { get; private set; }
	

	public event Action<GameCharacter> onDead;

	private GameCarriedObject _carryingTarget;


	void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		exp = 0;

		_levelData = Setting.characterLevels[0];
		_nextLevelData = Setting.characterLevels[1];

		UpdateSize();
	}

	void Update()
	{
		if (_carryingTarget != null)
		{
			_carryingTarget.transform.position = transform.position + Vector3.left /2;
		}
	}

	public void SetUp(Const.Side side)
	{
		this.side = side;
	}

	public void Move(Vector3 direction)
	{
		if (isCarried) return;
		_rigidbody2D.MovePosition(transform.position + direction);
	}

	public void MoveUp()
	{
		if (isCarried) return;
		Move(Vector3.up * _speed * Time.fixedDeltaTime);
	}

	public void MoveDown()
	{
		if (isCarried) return;
		Move(Vector3.down * _speed * Time.fixedDeltaTime);
	}

	public void MoveLeft()
	{
		if (isCarried) return;
		Move(Vector3.left * _speed * Time.fixedDeltaTime);
	}

	public void MoveRight()
	{
		if (isCarried) return;
		Move(Vector3.right * _speed * Time.fixedDeltaTime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		var food = other.GetComponent<GameFood>();

		if (food != null)
		{
			EatFood(food);
		}

		var character = other.GetComponentInParent<GameCharacter>();

		if (character != null)
		{
			EatCharacter(character);
		}

		var egg = other.GetComponent<GameEgg>();

		if (egg != null)
		{
			EatEgg(egg);
		}
	}

	private void EatFood(GameFood food)
	{
		if (food.isCarried) return;
		AddExp(1);
		food.OnEat();
	}

	private void EatCharacter(GameCharacter character)
	{
		if (character.side == this.side || character.isCarried) return;

		if (this.level > character.level)
		{
			AddExp(5);
			character.Kill();
		}

	}

	private void EatEgg(GameEgg egg)
	{
		if (egg.side == this.side|| egg.isCarried) return;
		AddExp(3);
		egg.OnEat();
	}

	private void AddExp(int add)
	{
		this.exp += add;

		currentLayEggCount += add;

		if (_levelData.layEggCount <= currentLayEggCount && _levelData.layEggCount > 0)
		{
			currentLayEggCount = 0;
			LayEgg();
		}

		if (_nextLevelData == null) return;

		int nextExp = _nextLevelData.exp;
		if (nextExp <= this.exp)
		{
			this.exp = 0;
			_levelData = _nextLevelData;
			_nextLevelData = Setting.characterLevels.Find(m => m.level > _levelData.level);

			UpdateSize();
		}
	}

	private void UpdateSize()
	{
		var rate = (float)1f / (float)Setting.characterLevels.Count;
		transform.localScale = Vector3.one * rate * _levelData.level;
	}

	private void LayEgg()
	{
		GameManager.instance.GenerateEgg(transform.position.x, transform.position.y, side);
	}

	public void Kill()
	{
		if (onDead != null)
		{
			onDead(this);
		}

		Destroy(this.gameObject);
	}

	public void MoveTo(Vector3 position, Action onComplete = null)
	{
		if (isCarried)
		{
			onComplete();
			return;
		}

		var distance = Vector3.Distance(position, this.transform.position);
		var duration = distance / _speed;

		_rigidbody2D.DOMove(position, duration, false)
		.SetEase(Ease.Linear)
		.OnComplete(
			() =>
			{
				if (onComplete != null)
				{
					onComplete();
				}
			}
		);
	}

	public void StartCarry()
	{
		var mask = LayerMask.GetMask(Const.layerNameCharacter, Const.layerNameEgg, Const.layerNameFood);
		var colliders = Physics.OverlapBox(transform.position, Vector3.one / 2, Quaternion.Euler(0, 0, 0), mask);
		
		var list = new List<Collider>(colliders);

		list.Sort(delegate(Collider a, Collider b)
		{
			return 
				Vector3.Distance(a.transform.position, transform.position)
				.CompareTo(
				Vector3.Distance(b.transform.position, transform.position));
		});

		foreach (var col in list)
		{
			if (col.gameObject == this.gameObject) continue;

			var character = col.GetComponent<GameCharacter>();

			if (character != null)
			{
				if (character.level < this.level)
				{
					character.OnCarriedStart(this);
					_carryingTarget = character;
					return;
				}
			}

			var obj = col.GetComponent<GameCarriedObject>();

			if (obj != null)
			{
				obj.OnCarriedStart(this);
				_carryingTarget = obj;
			}
		}
	}

	public void EndCarry()
	{
		if (_carryingTarget == null) return;
		_carryingTarget.OnCarriedEnd(this);
		_carryingTarget = null;
	}

	public override void OnCarriedStart(GameCharacter character)
	{
		isCarried = true;
		_rigidbody2D.Sleep();
	}

	public override void OnCarriedEnd(GameCharacter character)
	{
		isCarried = false;
	}
}