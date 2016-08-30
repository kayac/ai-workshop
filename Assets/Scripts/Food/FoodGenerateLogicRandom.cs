using UnityEngine;

public class FoodGenerateLogicRandom : FoodGenerateLogicBase
{
	private float _currentTime;

	private float _generateTime = 10f;

	private int _generateRate = 5;

	public override void OnInit()
	{
		SetFoods();
	}

	void Update()
	{
		_currentTime += Time.deltaTime;

		if (_generateTime <= _currentTime)
		{
			_currentTime = 0;
			SetFoods();
		}
	}

	private void SetFoods()
	{
		var map = GameManager.instance.map;
		var mapX = GameManager.instance.mapSizeX;
		var mapY = GameManager.instance.mapSizeY;

		var layerMask = LayerMask.GetMask(Const.layerNameCharacter, Const.layerNameEgg, Const.layerNameFood);

		for (int x = 0; x < mapX; x++)
		{
			for (int y = 0; y < mapY; y++)
			{
				var cell = map[x, y];
				
				if (cell.contentType != Const.MapCellType.None) continue;

				var pos = GameUtils.GetPosition(x, y);
				var col = Physics2D.OverlapBox(pos, Const.cellSizeVector2, 0, layerMask);

				if (col == null)
				{
					var isGenerate = Random.Range(0, _generateRate) == 0;
					if (isGenerate)
					{
						GameManager.instance.GenerateFood(x, y);
					}
				}
			}
		}
	}
}
