using UnityEngine;
using System.Collections;

public abstract class GameCarriedObject : MonoBehaviour
{

	public GameCharacter carringCharacter { get; protected set; }
	public bool isCarried { get { return carringCharacter != null; } }

	public abstract void OnCarriedStart(GameCharacter character);
	public abstract void OnCarriedEnd(GameCharacter character);
}
