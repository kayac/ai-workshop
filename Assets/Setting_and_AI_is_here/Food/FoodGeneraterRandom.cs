﻿using UnityEngine;

/// <summary>
/// 一定期間ごとにランダムに食べ物を配置する
/// </summary>
public class FoodGeneraterRandom : FoodGeneraterBase
{
	private float _currentTime;

	private float _generateTime = 10f;

	private int _generateRate = 5;

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
		var mapX = Setting.mapSizeX;
		var mapY = Setting.mapSizeY;

		var layerMask = LayerMask.GetMask(Const.layerNameCharacter, Const.layerNameEgg, Const.layerNameFood);

		for (int x = 0; x < mapX; x++)
		{
			for (int y = 0; y < mapY; y++)
			{
				var cell = map[x, y];
				
				if (cell != Const.MapCellType.None) continue;

				var pos = GameUtils.GetPosition(x, y);
				var col = Physics2D.OverlapBox(pos, Const.cellSizeVector2, 0, layerMask);

				if (col == null)
				{
					var isGenerate = Random.Range(0, _generateRate) == 0;
					if (isGenerate)
					{

						var r = Random.Range(0, 10);

						var myFoodType = Const.FoodType.Normal;

						if (r == 0) myFoodType = Const.FoodType.Super;

						if (r == 1 || r == 2) myFoodType = Const.FoodType.Egg;

						GameManager.instance.GenerateFood(x, y, myFoodType);
					}
				}
			}
		}
	}
}
