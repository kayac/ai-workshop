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
				var cellType = GameMapCell.CellType.None;
				var hasFood = false;
				switch (r)
				{
					case 0:
						cellType = GameMapCell.CellType.Wall;
						break;

					case 1:
						cellType = GameMapCell.CellType.River;
						break;
				}
				
				if (cellType == GameMapCell.CellType.None)
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
