using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

/// <summary>
/// キャラクター
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GameCharacter : MonoBehaviour
{
	/// <summary>
	/// 移動速度(毎秒あたり)
	/// </summary>
	[SerializeField]
	private float _speed;
	
	private Rigidbody _rigidbody;

	/// <summary>
	/// 現在のレベル
	/// </summary>
	/// <returns></returns>
	public int level { get; private set; } 

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

	private TextMesh _text;

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		exp = 0;
		level = 1;

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
	}

	private void EatFood(GameFood food)
	{
		exp++;

		int nextExp = 0;
		switch (level)
		{
			case 1: nextExp = 5; break;
			case 2: nextExp = 10; break;
			case 3: nextExp = 15; break;
		}

		if (nextExp <= exp)
		{
			Debug.Log("level up");
			exp = 0;
			level++;

			_text.text = level.ToString();
		}

		food.OnEat();
	}

	private void EatCharacter(GameCharacter character)
	{
		if (character.side == this.side) return;

		if (this.level > character.level)
		{
			character.Kill();
		}

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