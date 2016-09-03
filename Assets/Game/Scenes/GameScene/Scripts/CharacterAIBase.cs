using UnityEngine;
using System.Collections;

public abstract class CharacterAIBase : MonoBehaviour
{
	protected Character _character;

	void Awake()
	{
		_character = GetComponent<Character>();
	}
}
