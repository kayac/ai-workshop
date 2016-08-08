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

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
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

	public void MoveTo(Vector3 position, Action onComplete = null)
	{
		Debug.Log("pos:" + position.x + " ," + position.y);
		var distance = Vector3.Distance(position, this.transform.position);
		var duration = distance / _speed;
		_rigidbody.DOMove(position, duration, false)
		.SetEase(Ease.Linear)
		.OnComplete(
			() =>
			{
				_rigidbody.MovePosition(position);
				if (onComplete != null)
				{
					onComplete();
				}
			}
		);
	}
}