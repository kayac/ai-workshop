using UnityEngine;
using System.Collections;

public class GameFood : GameCarriedObject
{

	public override void OnCarriedStart(GameCharacter character)
	{
		isCarried = true;
	}

	public override void OnCarriedEnd(GameCharacter character)
	{
		isCarried = false;
	}

	public void OnEat()
	{
		Destroy(this.gameObject);
	}
}
