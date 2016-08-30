using UnityEngine;
using System.Collections;

public class GameFood : GameCarriedObject
{
	[SerializeField]
	private SpriteRenderer _renderer;
	
	public Const.FoodType foodType { get; private set; }

	public void SetUp(Const.FoodType foodType)
	{
		this.foodType = foodType;
	}

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
