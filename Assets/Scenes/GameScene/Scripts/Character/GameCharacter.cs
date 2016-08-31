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
	private enum Status
	{
		Default,
		Inhale,
		Chew,
		Carry,
		Carried,
		Dead
	}

	#region GameObject
	[SerializeField]
	private SpriteRenderer _spriteRederer;

	[Header("sprite index: 0")]
	[SerializeField]
	private Sprite _defaultStandSprite;

	[Header("sprite index: 1")]
	[SerializeField]
	private Sprite _defaultWalk1Sprite;

	[Header("sprite index: 2")]
	[SerializeField]
	private Sprite _defaultWalk2Sprite;

	[Header("sprite index: 10")]
	[SerializeField]
	private Sprite _inhaleStandSprite;

	[Header("sprite index: 11")]
	[SerializeField]
	private Sprite _inhaleWalk1Sprite;

	[Header("sprite index: 12")]
	[SerializeField]
	private Sprite _inhaleWalk2Sprite;

	[Header("sprite index: 20")]
	[SerializeField]
	private Sprite _chewStand1Sprite;

	[Header("sprite index: 21")]
	[SerializeField]
	private Sprite _chewStand2Sprite;

	[Header("sprite index: 22")]
	[SerializeField]
	private Sprite _chewWalk1Sprite;

	[Header("sprite index: 23")]
	[SerializeField]
	private Sprite _chewWalk2Sprite;

	[Header("sprite index: 30")]
	[SerializeField]
	private Sprite _carryStandSptire;

	[Header("sprite index: 31")]
	[SerializeField]
	private Sprite _carryWalk1Sprite;

	[Header("sprite index: 32")]
	[SerializeField]
	private Sprite _carryWalk2Sprite;

	[Header("sprite index: 40")]
	[SerializeField]
	private Sprite _carriedSprite;

	[Header("sprite index: 50")]
	[SerializeField]
	private Sprite _deadSprite;
	
	[SerializeField]
	private SpriteRenderer _hair;

	[SerializeField]
	private ParticleSystem _superModeParticle;

	[SerializeField]
	private Transform _hairFireRoot;

	#endregion

	/// <summary>
	/// 移動速度(毎秒あたり)
	/// </summary>
	[Header("移動速度(毎秒あたり)")]
	[SerializeField]
	private float _speed;

	/// <summary>
	/// 表示するスプライトの番号
	/// タイムラインアニメーションから操作できるようにパブリックになってます
	/// </summary>
	public float spriteIndex;

	private Rigidbody2D _rigidbody2D;

	private Animator _animator;

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

	/// <summary>
	/// 現在運搬しているモノ
	/// </summary>
	private GameCarriedObject _carryingTarget;

	private Status _status;

	private bool _isMoving;
	
	void Awake()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();

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
				_superModeParticle.gameObject.SetActive(false);
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

	/// <summary>
	/// スプライトの表示を更新する
	/// </summary>
	private void UpdateSprite()
	{
		Sprite s = null;

		var i = Mathf.RoundToInt(spriteIndex);

		switch (i)
		{
			case 0: s = _defaultStandSprite; break;
			case 1: s = _defaultWalk1Sprite; break;
			case 2: s = _defaultWalk2Sprite; break;

			case 10: s = _inhaleStandSprite; break;
			case 11: s = _inhaleWalk1Sprite; break;
			case 12: s = _inhaleWalk2Sprite; break;

			case 20: s = _chewStand1Sprite; break;
			case 21: s = _chewStand2Sprite; break;
			case 22: s = _chewWalk1Sprite; break;
			case 23: s = _chewWalk2Sprite; break;

			case 30: s = _carryStandSptire; break;
			case 31: s = _carryWalk1Sprite; break;
			case 32: s = _carryWalk2Sprite; break;

			case 40: s = _carriedSprite; break;

			case 50: s = _deadSprite; break;

			default: s = _defaultStandSprite; break;
		}

		if (_spriteRederer.sprite != s)  _spriteRederer.sprite = s;
	}

	private void ChangeAnimation(Status status)
	{
		ChangeAnimation(status, _isMoving);
	}

	private void ChangeAnimation(bool isMoving)
	{
		ChangeAnimation(_status, isMoving);
	}

	private void ChangeAnimation(Status status, bool isMoving)
	{
		if (_status == status && _isMoving == isMoving)
		{
			return;
		}

		_status = status;
		_isMoving = isMoving;

		switch (_status)
		{
			case Status.Default:
				if (_isMoving)
				{
					_animator.Play("DefaultWalk");
				}
				else
				{
					_animator.Play("DefaultStand");
				}
				return;

			case Status.Inhale:
				if (_isMoving)
				{
					_animator.Play("InhaleWalk");
				}
				else
				{
					_animator.Play("InhaleStand");
				}
				return;

			case Status.Chew:
				if (_isMoving)
				{
					_animator.Play("ChewWalk");
				}
				else
				{
					_animator.Play("ChewStand");
				}
				return;

			case Status.Carry:
				if (_isMoving)
				{
					_animator.Play("CarryWalk");
				}
				else
				{
					_animator.Play("CarryStand");
				}
				return;

			case Status.Carried:
				_animator.Play("Carried");
				return;

			case Status.Dead:
				_animator.Play("Dead");
				return;
		}
	}

	/// <summary>
	/// レベルに応じてサイズを変える
	/// </summary>
	private void UpdateSize()
	{
		if (Application.isPlaying)
		{
			var rate = (float)1f / (float)Setting.characterLevels.Count;
			transform.localScale = Vector3.one * rate * _levelData.level;
		}
	}

	public void SetUp(Const.Side side)
	{
		this.side = side;
	}
	
	/// <summary>
	/// 移動
	/// </summary>
	/// <param name="direction"></param>
	public void Move(Vector3 direction)
	{
		if (isCarried) return;
		_rigidbody2D.MovePosition(transform.position + direction);

		ChangeAnimation(true);
	}

	/// <summary>
	/// 上に移動
	/// </summary>
	public void MoveUp()
	{
		if (isCarried) return;
		Move(Vector3.up * _speed * Time.fixedDeltaTime);
	}

	/// <summary>
	/// 下に移動
	/// </summary>
	public void MoveDown()
	{
		if (isCarried) return;
		Move(Vector3.down * _speed * Time.fixedDeltaTime);
	}

	/// <summary>
	/// 左に移動
	/// </summary>
	public void MoveLeft()
	{
		if (isCarried) return;
		Move(Vector3.left * _speed * Time.fixedDeltaTime);
	}

	/// <summary>
	/// 右に移動
	/// </summary>
	public void MoveRight()
	{
		if (isCarried) return;
		Move(Vector3.right * _speed * Time.fixedDeltaTime);
	}

	public void CancenMove()
	{
		ChangeAnimation(false);
	}


		/// <summary>
	/// ある地点に向かって移動する
	/// </summary>
	/// <param name="position"></param>
	/// <param name="onComplete"></param>
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
				ChangeAnimation(false);
			}
		);

		ChangeAnimation(true);
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

	/// <summary>
	/// スーパー状態を開始する
	/// </summary>
	private void StartSuperMode()
	{
		isSuperMode = true;
		superModeRemainTime = Setting.superModeTime;
		_superModeParticle.gameObject.SetActive(true);
	}

	/// <summary>
	/// 食べ物を食べる
	/// </summary>
	/// <param name="food"></param>
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

	/// <summary>
	/// キャラクターを食べる
	/// </summary>
	/// <param name="character"></param>
	private void EatCharacter(GameCharacter character)
	{
		if (character.side == this.side || character.isCarried || this.isCarried || character.isSuperMode) return;

		if (this.level > character.level || this.isSuperMode)
		{
			Eat(Setting.expEatCharacter, Setting.layEggCountEatCharacter);
			character.Kill();
		}

	}

	/// <summary>
	/// 卵を食べる
	/// </summary>
	/// <param name="egg"></param>
	private void EatEgg(GameEgg egg)
	{
		if (egg.side == this.side|| egg.isCarried) return;
		Eat(Setting.expEatEgg, Setting.layEggCountEatEgg);
		egg.OnEat();
	}

	/// <summary>
	/// 食べる処理
	/// </summary>
	/// <param name="addExp"></param>
	/// <param name="layEggCount"></param>
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

	/// <summary>
	/// 卵を産む
	/// </summary>
	private void LayEgg()
	{
		currentLayEggCount = 0;
		GameManager.instance.GenerateEgg(transform.position.x, transform.position.y, side);
	}

	/// <summary>
	/// 殺す
	/// </summary>
	public void Kill()
	{
		if (onDead != null)
		{
			onDead(this);
		}

		Destroy(this.gameObject);
	}



	/// <summary>
	/// 運搬を開始する
	/// </summary>
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
					ChangeAnimation(Status.Carry);
					return;
				}
			}
			else
			{

				var obj = col.GetComponent<GameCarriedObject>();

				if (obj != null)
				{
					obj.OnCarriedStart(this);
					_carryingTarget = obj;
					ChangeAnimation(Status.Carry);
				}
			}
		}
	}

	/// <summary>
	/// 運搬を終了する
	/// </summary>
	public void EndCarry()
	{
		if (_carryingTarget == null) return;
		_carryingTarget.OnCarriedEnd(this);
		_carryingTarget = null;
		ChangeAnimation(Status.Default);
	}

	public bool IsCarring()
	{
		return _carryingTarget != null;
	}

	/// <summary>
	/// 運搬された時に呼ばれる
	/// </summary>
	/// <param name="character"></param>
	public override void OnCarriedStart(GameCharacter character)
	{
		isCarried = true;
		_rigidbody2D.Sleep();
		ChangeAnimation(Status.Carried);
	}

	/// <summary>
	/// 運搬され終わった時に呼ばれる
	/// </summary>
	/// <param name="character"></param>
	public override void OnCarriedEnd(GameCharacter character)
	{
		isCarried = false;
		ChangeAnimation(Status.Default);
	}
}