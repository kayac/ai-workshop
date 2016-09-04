using UnityEngine;
using System.Collections;

/// <summary>
/// ユーティリティ
/// </summary>
public static class GameUtils
{
	/// <summary>
	/// マスの座標を座標をUnityの座標に変換する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static Vector2 GetPosition(int x, int y)
	{
		return new Vector2(Const.cellSizeX * x, Const.cellSizeY * y);
	}
}
