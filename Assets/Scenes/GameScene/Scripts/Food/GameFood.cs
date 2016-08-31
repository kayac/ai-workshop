using UnityEngine;
using DG.Tweening;

public class GameFood : GameCarriedObject
{
	[SerializeField]
	private SpriteRenderer _renderer;

	[SerializeField]
	private Sprite _normalSprite;

	[SerializeField]
	private Sprite _layEggSprite;

	[SerializeField]
	private Sprite _superSprite;

	public Const.FoodType foodType { get; private set; }

	void OnEnable()
	{
		PlayAppear();
	}

	public void SetUp(Const.FoodType foodType)
	{
		this.foodType = foodType;

		var mySprite = _normalSprite;

		switch(foodType)
		{
			case Const.FoodType.Normal: mySprite = _normalSprite; break;
			case Const.FoodType.Egg   : mySprite = _layEggSprite; break;
			case Const.FoodType.Super : mySprite = _superSprite; break;
		}

		_renderer.sprite = mySprite;
	}

	public void PlayAppear()
	{
		_renderer.transform.localScale = Vector3.zero;
		_renderer.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
	}

	public override void OnCarriedStart(GameCharacter character)
	{
		isCarried = true;
	}

	public override void OnCarriedEnd(GameCharacter character)
	{
		isCarried = false;
	}

	public void OnEat(GameCharacter character)
	{
		
		this.enabled = false;
		GetComponent<Collider2D>().enabled = false;

		transform.position = new Vector3(
			transform.position.x,
			transform.position.y,
			Const.eatAnimationStartPositionZ
		);

		var duration = 0.25f;

		transform.DOMove(character.GetInhaleWorldPosition(), duration).OnComplete(()=>
		{
			Destroy(this.gameObject);
		});

		transform.DOScale(Vector3.zero, duration);
	}
}
