using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameCharacter))]
public class GameCharacterController : MonoBehaviour
{
	private KeyCode? _prevKeyCode;

	private static readonly KeyCode[] _moveKeyCodes = new KeyCode[]{KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow};

	private GameCharacter _character;

	void Awake()
	{
		_character = GetComponent<GameCharacter>();
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			_character.StartCarry();
		}
		else
		{
			_character.EndCarry();
		}
	}

	void FixedUpdate()
	{
		ProcessMove();

		
	}

	private void ProcessMove()
	{
		if (GameManager.instance.isGameOver) return;

		if (Input.GetKey(KeyCode.UpArrow)
			|| Input.GetKey(KeyCode.DownArrow)
			|| Input.GetKey(KeyCode.LeftArrow)
			|| Input.GetKey(KeyCode.RightArrow)
		)
		{
			for (int i = 0; i < _moveKeyCodes.Length; i++)
			{
				var keyCode = _moveKeyCodes[i];
				if (IsChangeDirection(keyCode) && Input.GetKey(keyCode))
				{
					Move(keyCode);
					return;
				}
			}

			for (int i = 0; i < _moveKeyCodes.Length; i++)
			{
				var keyCode = _moveKeyCodes[i];
				if (Input.GetKey(keyCode))
				{
					Move(keyCode);
					return;
				}
			}
		}
		else
		{
			_prevKeyCode = null;
			_character.CancenMove();
		}
	}

	private void Move(KeyCode keycode)
	{
		_prevKeyCode = keycode;

		switch(keycode)
		{
			case KeyCode.UpArrow: _character.MoveUp(); return;
			case KeyCode.DownArrow: _character.MoveDown(); return;
			case KeyCode.LeftArrow: _character.MoveLeft(); return;
			case KeyCode.RightArrow: _character.MoveRight(); return;
		}
	}

	private bool IsChangeDirection(KeyCode keyCode)
	{
		return keyCode != _prevKeyCode;
	}
}
