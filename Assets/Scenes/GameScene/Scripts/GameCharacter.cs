using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GameCharacter : MonoBehaviour
{
	[SerializeField]
	private float _speed;

	private Rigidbody2D _rigidbody;

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			_rigidbody.MovePosition((Vector2)transform.position + Vector2.left * Time.fixedDeltaTime * _speed);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			_rigidbody.MovePosition((Vector2)transform.position + Vector2.right * Time.fixedDeltaTime * _speed);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			_rigidbody.MovePosition((Vector2)transform.position + Vector2.up * Time.fixedDeltaTime * _speed);
		}

		if (Input.GetKey(KeyCode.DownArrow))
		{
			_rigidbody.MovePosition((Vector2)transform.position + Vector2.down * Time.fixedDeltaTime * _speed);
		}
	}
}
