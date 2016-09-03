using UnityEngine;
using System.Collections;
using DG.Tweening;
/// <summary>
/// 卵 : 一定時間で孵化する
/// </summary>
public class Egg : CarriedObject
{
	
	[SerializeField]
	private float _hatchTime = 10;

	/// <summary>
	/// 産まれるまでの時間(秒)
	/// </summary>
	public float hatchTime { get {return _hatchTime; } }

	/// <summary>
	/// 現在経過している時間(秒)
	/// </summary>
	private float _currentTime;

	/// <summary>
	/// 既に孵化しているか
	/// </summary>
	/// <returns></returns>
	public bool alreadyHatch { get; private set; }

	/// <summary>
	/// 味方の卵/敵の卵
	/// </summary>
	/// <returns></returns>
	public Const.Side side { get; private set; }

	/// <summary>
	/// animator
	/// </summary>
	private Animator _animator;

	void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void SetUp(Const.Side side)
	{
		this.side = side;
	}

	void Update()
	{
		if (!alreadyHatch)
		{
			_currentTime += Time.deltaTime;

			var animTime = hatchTime / 4;

			if (animTime * 4 < _currentTime)
			{
				_animator.speed = 4;
			}
			else if(animTime * 3 < _currentTime)
			{
				_animator.speed = 3;
			}
			else if (animTime * 2 < _currentTime)
			{
				_animator.speed = 2;
			}
			else
			{
				_animator.speed = 1;
			}
			
			if (hatchTime <= _currentTime)
			{
				GameManager.instance.OnHatchEgg(this);
				alreadyHatch = true;
				OnHatch();
			}
		}
	}

	public void OnEat(Character character)
	{
		DOTween.Kill(transform, false);

		GameManager.instance.OnEatEgg(this);

		transform.position = new Vector3(
			transform.position.x,
			transform.position.y,
			Const.eatAnimationStartPositionZ
		);

		var duration = 0.25f;

		transform.DOMove(character.GetInhaleWorldPosition(), duration).OnComplete(()=>
		{
			Destroy(this.gameObject);
		});

		transform.DOScale(transform.localScale / 2, duration);

		this.enabled = false;
	}

	private void OnHatch()
	{
		_animator.speed = 1;
		_animator.Play("Hatch");

		if (carringCharacter != null)
		{
			carringCharacter.EndCarry();
		}

		Destroy(this.gameObject, 2);
		this.enabled = false;
	}

	public override void OnCarriedStart(Character character)
	{
		carringCharacter = character;
	}

	public override void OnCarriedEnd(Character character)
	{
		carringCharacter = null;
	}
}
