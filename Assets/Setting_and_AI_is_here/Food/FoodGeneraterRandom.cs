using UnityEngine;

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
		// 地図を取得
		var map = GameManager.instance.map;

		// マップのサイズを取得
		var mapSizeX = Setting.mapSizeX;
		var mapSizeY = Setting.mapSizeY;

		// レイヤーを取得
		var layerMask = LayerMask.GetMask(Const.layerNameCharacter, Const.layerNameEgg, Const.layerNameFood);

		// マスを１つずつ見ていく
		for (int x = 0; x < mapSizeX; x++)
		{
			for (int y = 0; y < mapSizeY; y++)
			{
				var cell = map[x, y];

				// マップの座標が平原ではなければ次のマスを見る	
				if (cell != Const.MapCellType.None) continue;

				// マスの座標からワールド座標を取得
				var pos = GameUtils.GetPosition(x, y);

				// 食べ物配置予定地にキャタクター、卵、食べ物がないかを確認する
				var col = Physics2D.OverlapBox(pos, Const.cellSizeVector2, 0, layerMask);

				// 予定地に何も存在しなかった場合
				if (col == null)
				{
					// 乱数で生成する食べ物の種類を選ぶ
					var isGenerate = Random.Range(0, _generateRate) == 0;
					if (isGenerate)
					{

						var r = Random.Range(0, 10);

						var myFoodType = Const.FoodType.Normal;

						if (r == 0) myFoodType = Const.FoodType.Super;

						if (r == 1 || r == 2) myFoodType = Const.FoodType.Egg;

						// 食べ物を配置
						GameManager.instance.GenerateFood(x, y, myFoodType);
					}
				}
			}
		}
	}
}
