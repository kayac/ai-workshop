using UnityEngine;

public class GameMapGeneratorRandom : GameMapGeneratorBase
{
	public override GameMapCell[,] Generate(int sizeX, int sizeY)
	{
		var map = new GameMapCell[sizeX, sizeY];

		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				var r = Random.Range(0, 10);
				var cellType = Const.MapCellType.None;
				var hasFood = false;
				switch (r)
				{
					case 0:
						cellType = Const.MapCellType.Wall;
						break;

					case 1:
						cellType = Const.MapCellType.River;
						break;
				}
				
				if (cellType == Const.MapCellType.None)
				{
					var f = Random.Range(0, 5);
					if (f == 0)
					{
						hasFood = true;
					}
				}

				var cell = new GameMapCell(x, y, cellType, hasFood);
				map[x, y] = cell;
			}
		}

		return map;
	}
}
