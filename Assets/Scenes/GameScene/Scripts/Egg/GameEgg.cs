using UnityEngine;
using System.Collections;

/// <summary>
/// 卵 : 一定時間で孵化する
/// </summary>
public class GameEgg : GameCarriedObject
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
				GameManager.instance.GenerateCharacter(transform.position.x, transform.position.y, side);
				alreadyHatch = true;
				OnHatch();
			}
		}
	}

	public void OnEat()
	{
		Destroy(this.gameObject);
	}

	private void OnHatch()
	{
		_animator.speed = 1;
		_animator.Play("Hatch");
		Destroy(this.gameObject, 2);
		this.enabled = false;
	}

	public override void OnCarriedStart(GameCharacter character)
	{
		isCarried = true;
	}

	public override void OnCarriedEnd(GameCharacter character)
	{
		isCarried = false;
	}
}
