using UnityEngine;
using System.Collections;

public abstract class GameCharacterAIBase : MonoBehaviour
{
	protected GameCharacter _character;

	void Awake()
	{
		_character = GetComponent<GameCharacter>();
	}
}
