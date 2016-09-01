using UnityEngine;
using System.Collections;

public static class GameUtils
{
	public static Vector2 GetPosition(int x, int y)
	{
		return new Vector2(Const.cellSizeX * x, Const.cellSizeY * y);
	}
}
