using UnityEngine;
using System.Collections;

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

				/*
				 01234567890123456789
				*/
				"____________________", // 9
				"____________________", // 8
				"____________________", // 7
				"____________________", // 6
				"____________________", // 5
				"____________________", // 4
				"____________________", // 3
				"____________________", // 2
				"____________________", // 1
				"__________P_________", // 0
				"____________________", // 9
				"____________________", // 8
				"____________________", // 7
				"____________________", // 6
				"____________________", // 5
				"____________________", // 4
				"____________________", // 3
				"____________________", // 2
				"____________________", // 1
				"____________________", // 0
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
					default: p = Const.GameStagePreset.StageGround;			break;
				}

				preset[x, sizeY - 1 - y] = p;
			}
		}

		return preset;
	}
}
