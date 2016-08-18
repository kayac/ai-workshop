using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// キャラクター
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GameCharacter : MonoBehaviour
{
	[System.Serializable]
	public class LevelData
	{
		[Header("レベル")]
		[SerializeField]
		private int _level;
		public int level { get { return _level; } }

		[Header("このレベルに到達するまでの経験値")]
		[SerializeField]
		private int _exp;
		public int exp { get { return _exp; } }

		[Header("卵を産むまでのカウント -1を指定すると産まない")]
		[SerializeField]
		private int _layEggCount;
		public int layEggCount { get { return _layEggCount; } }
	}

	/// <summary>
	/// 移動速度(毎秒あたり)
	/// </summary>
	[Header("移動速度(毎秒あたり)")]
	[SerializeField]
	private float _speed;
	
	/// <summary>
	/// レベル毎のパラメーター一覧
	/// </summary>
	[Header("レベル毎のパラメーター一覧")]
	[SerializeField]
	private List<LevelData> _levels;

	private Rigidbody _rigidbody;

	private LevelData _levelData;

	private LevelData _nextLevelData;


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

	private TextMesh _text;

	void OnValidate()
	{
		_levels.Sort(delegate(LevelData a, LevelData b)
		{
			return a.level - b.level;
		}
		);
	}

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		exp = 0;

		_levelData = _levels[0];
		_nextLevelData = _levels[1];

		_text = GetComponentInChildren<TextMesh>();

		_text.text = level.ToString();
	}

	public void SetUp(Const.Side side)
	{
		this.side = side;
	}

	public void Move(Vector3 direction)
	{
		_rigidbody.MovePosition(transform.position + direction);
	}

	public void MoveUp()
	{
		Move(Vector3.up * _speed * Time.fixedDeltaTime);
	}

	public void MoveDown()
	{
		Move(Vector3.down * _speed * Time.fixedDeltaTime);
	}

	public void MoveLeft()
	{
		Move(Vector3.left * _speed * Time.fixedDeltaTime);
	}

	public void MoveRight()
	{
		Move(Vector3.right * _speed * Time.fixedDeltaTime);
	}

	void OnTriggerEnter(Collider other)
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
		AddExp(1);
		food.OnEat();
	}

	private void EatCharacter(GameCharacter character)
	{
		if (character.side == this.side) return;

		if (this.level > character.level)
		{
			AddExp(5);
			character.Kill();
		}

	}

	private void EatEgg(GameEgg egg)
	{
		if (egg.side == this.side) return;
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
			_nextLevelData = _levels.Find(m => m.level > _levelData.level);
			_text.text = level.ToString();
		}
	}

	private void LayEgg()
	{
		GameManager.instance.GenerateEgg(transform.position.x, transform.position.y, side);
	}

	public void Kill()
	{
		Destroy(this.gameObject);
	}

	public void MoveTo(Vector3 position, Action onComplete = null)
	{
		var distance = Vector3.Distance(position, this.transform.position);
		var duration = distance / _speed;
		_rigidbody.DOMove(position, duration, false)
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
}