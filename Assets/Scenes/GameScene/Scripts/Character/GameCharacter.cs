using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// キャラクター
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[ExecuteInEditMode]
public class GameCharacter : GameCarriedObject
{

	[SerializeField]
	private SpriteRenderer _spriteRederer;

	[SerializeField]
	private Sprite _standSprite;

	[SerializeField]
	private Sprite _walk1Sprite;

	[SerializeField]
	private Sprite _walk2Sprite;

	/// <summary>
	/// 移動速度(毎秒あたり)
	/// </summary>
	[Header("移動速度(毎秒あたり)")]
	[SerializeField]
	private float _speed;

	[SerializeField]
	private SpriteRenderer _hair;

	public float spriteIndex;

	[SerializeField]
	private Transform _hairFireRoot;

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

	/// <summary>
	/// 最大寿命
	/// </summary>
	/// <returns></returns>
	public float maxLifeTime { get; private set; }

	/// <summary>
	/// 寿命までの残り時間
	/// </summary>
	/// <returns></returns>
	public float lifeTime { get; private set ; }
	
	/// <summary>
	/// 死亡した時に呼ばれるイベント
	/// </summary>
	public event Action<GameCharacter> onDead;

	/// <summary>
	/// スーパー状態
	/// </summary>
	public bool isSuperMode { get; private set; }

	/// <summary>
	/// スーパー状態の残り時間
	/// </summary>
	/// <returns></returns>
	public float superModeRemainTime { get; private set;}

	private GameCarriedObject _carryingTarget;

	void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		exp = 0;

		_levelData = Setting.characterLevels[0];
		_nextLevelData = Setting.characterLevels[1];

		maxLifeTime = Setting.characterMaxLifeTime;
		lifeTime = maxLifeTime;

		UpdateSize();
	}

	void Update()
	{
		UpdateSprite();

		if (_carryingTarget != null)
		{
			_carryingTarget.transform.position = transform.position + Vector3.left /2;
		}

		if (isSuperMode)
		{
			superModeRemainTime -= Time.deltaTime;
			if (superModeRemainTime <= 0)
			{
				superModeRemainTime = -1;
				isSuperMode = false;
			}
		}

		ProcessLifeTime();
	}

	void ProcessLifeTime()
	{
		lifeTime -= Time.deltaTime;

		if (lifeTime < 0)
		{
			Kill();
		}
		else
		{
			var rate = lifeTime / maxLifeTime;
			
			_hair.transform.localScale = new Vector3(1, rate, 1);
			_hairFireRoot.position = new Vector3(_hairFireRoot.position.x, _hair.bounds.max.y, _hairFireRoot.position.z);
		}
	}

	void UpdateSprite()
	{
		Sprite s = null;

		switch ((int)spriteIndex)
		{
			case 0:
				s = _standSprite;
				break;

			case 1:
				s = _walk1Sprite;
				break;

			case 2:
				s = _walk2Sprite;
				break;

			default:
				s = _standSprite;
				break;
		}
		_spriteRederer.sprite = s;
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

	private void StartSuperMode()
	{
		isSuperMode = true;
		superModeRemainTime = Setting.superModeTime;
	}

	private void EatFood(GameFood food)
	{
		if (food.isCarried) return;
		
		switch (food.foodType)
		{
			case Const.FoodType.Super:
				StartSuperMode();
				break;

			case Const.FoodType.Egg:
				LayEgg();
				break;
		}

		Eat(Setting.expEatFood, Setting.layEggCountEatFood);
		food.OnEat();
	}

	private void EatCharacter(GameCharacter character)
	{
		if (character.side == this.side || character.isCarried || this.isCarried || character.isSuperMode) return;

		if (this.level > character.level || this.isSuperMode)
		{
			Eat(Setting.expEatCharacter, Setting.layEggCountEatCharacter);
			character.Kill();
		}

	}

	private void EatEgg(GameEgg egg)
	{
		if (egg.side == this.side|| egg.isCarried) return;
		Eat(Setting.expEatEgg, Setting.layEggCountEatEgg);
		egg.OnEat();
	}

	private void Eat(int addExp, int layEggCount)
	{
		this.exp += addExp;

		currentLayEggCount += layEggCount;

		if (_levelData.layEggCount <= currentLayEggCount && _levelData.layEggCount > 0)
		{
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
		currentLayEggCount = 0;
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
		var colliders = Physics2D.OverlapBoxAll(transform.position, Vector3.one / 2, mask);
		
		var list = new List<Collider2D>(colliders);

		list.Sort(delegate(Collider2D a, Collider2D b)
		{
			return 
				Vector2.Distance(a.transform.position, transform.position)
				.CompareTo(
				Vector2.Distance(b.transform.position, transform.position));
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