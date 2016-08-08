using UnityEngine;
using System;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class GameCharacter : MonoBehaviour
{
	[SerializeField]
	private float _speed;

	private Rigidbody _rigidbody;

	public int level { get; private set; } 
	public int exp { get; private set; }

	public Const.Side side { get; private set; }

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		exp = 0;
		level = 1;
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
		}

		food.OnEat();
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