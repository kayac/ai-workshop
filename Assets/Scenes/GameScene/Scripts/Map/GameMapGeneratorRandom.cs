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
				switch (r)
				{
					case 0:
						cellType = Const.MapCellType.Wall;
						break;

					case 1:
						cellType = Const.MapCellType.River;
						break;
				}

				var cell = new GameMapCell(x, y, cellType);
				map[x, y] = cell;
			}
		}

		return map;
	}
}
