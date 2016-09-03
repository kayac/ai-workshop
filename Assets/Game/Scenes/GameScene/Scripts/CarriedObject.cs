using UnityEngine;
using System.Collections;

public abstract class CarriedObject : MonoBehaviour
{

	public Character carringCharacter { get; protected set; }
	public bool isCarried { get { return carringCharacter != null; } }

	public abstract void OnCarriedStart(Character character);
	public abstract void OnCarriedEnd(Character character);
}
