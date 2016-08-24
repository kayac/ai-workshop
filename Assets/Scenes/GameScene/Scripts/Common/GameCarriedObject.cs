using UnityEngine;
using System.Collections;

public abstract class GameCarriedObject : MonoBehaviour
{

	public bool isCarried { get; protected set; }

	public abstract void OnCarriedStart(GameCharacter character);
	public abstract void OnCarriedEnd(GameCharacter character);
}
