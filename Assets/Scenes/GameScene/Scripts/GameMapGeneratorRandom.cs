using UnityEngine;
using System.Collections;

public class GameMapGeneratorRandom : GameMapGeneratorBase
{
	public override void Generate(int sizeX, int sizeY)
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				var r = Random.Range(0, 10);

				if (r == 0)
				{

				}
			}
		}
	}
}
