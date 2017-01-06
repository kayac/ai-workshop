using UnityEngine;
using System.Collections;

/// <summary>
/// マッピングされたゲーム開始時のステージの情報を生成する
/// </summary>
public class GamePresetDesigned : GamePresetBase
{

	//
	//	_ : StageGround
	// 	w : StageWall
	// 	r : StageRiver,
	// 	P : CharacterPlayer,
	// 	A : CharacterOwn,
	// 	B : CharacterOpp,
	// 	1 : FoodNormal,
	// 	2 : FoodSuper,
	// 	3 : FoodEgg,
	// 	a : EggOwn,
	// 	b : EggOpp
	//

	protected virtual string[] mapStr
	{
		get
		{
			return new string[]
			{
				// ステージ端の枠は自動で生成されるのでなくてOK
				/*
				 01234567890123456789
				*/
				"2_111B11B1B1B1B1B1B2", // 9
				"1_111B11B1BB1BB1BB1B", // 8
				"1_111B11B11111111111", // 7
				"1_111B11B11333333333", // 6
				"w_wwwwwwwww333B3B333", // 5
				"w2_1111111w333333332", // 4
				"wwwwwwwww1wwwwwwwwww", // 3
				"1111_____1______1111", // 2
				"1111_____1______1111", // 1
				"1111____________1111", // 0
				"1111____111_____1111", // 9
				"1111____111_____1111", // 8
				"1111____111_____1111", // 7
				"1111___3___3____1111", // 6
				"rrrrrrrrr1rrrrrrrrrr", // 5
				"rrrrrrrrr1rrrrrrrrrr", // 4
				"rrrrrrrrr1rrrrrrrrrr", // 3
				"rrrrrrrrr1rrrrrrrrrr", // 2
				"rrrrrrrrr1rrrrrrrrrr", // 1
				"rrrrrrrrrPrrrrrrrrrr", // 0
				/*
				 01234567890123456789
				*/
			};
		}
	}

	public override Const.GameStagePreset[,] Generate(int sizeX, int sizeY)
	{
		var preset = new Const.GameStagePreset[sizeX, sizeY];

		for (int y = sizeY - 1; 0 <= y; y--)
		{
			for (int x = 0; x < sizeX; x++)
			{
				var s = mapStr[y].Substring(x, 1);
				var p = Const.GameStagePreset.StageGround;

				switch (s)
				{
					case "_": p = Const.GameStagePreset.StageGround;		break;
					case "w": p = Const.GameStagePreset.StageWall;			break;
					case "r": p = Const.GameStagePreset.StageRiver;			break;
					case "P": p = Const.GameStagePreset.CharacterPlayer;	break;
					case "A": p = Const.GameStagePreset.CharacterOwn;		break;
					case "B": p = Const.GameStagePreset.CharacterOpp;		break;
					case "1": p = Const.GameStagePreset.FoodNormal;			break;
					case "2": p = Const.GameStagePreset.FoodSuper;			break;
					case "3": p = Const.GameStagePreset.FoodEgg;			break;
					case "a": p = Const.GameStagePreset.EggOwn;				break;
					case "b": p = Const.GameStagePreset.EggOpp;				break;
					default:
						Debug.LogError(string.Format("x:{0}, y:{1} に不正な文字「{2}」が入力されています", x, y, s));
						p = Const.GameStagePreset.StageGround;
						break;
				}

				preset[x, (sizeY - 1) - y] = p;
			}
		}

		return preset;
	}
}
