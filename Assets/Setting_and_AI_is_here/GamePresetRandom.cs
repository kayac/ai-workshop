using UnityEngine;

public class GamePresetRandom : GamePresetBase
{
	public override Const.GameStagePreset[,] Generate(int sizeX, int sizeY)
	{
		var preset = new Const.GameStagePreset[sizeX, sizeY];

		// マップを作る
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				var p = Const.GameStagePreset.StageGround;

				int r = Random.Range(0, 10);

				switch(r)
				{
					case 0: p = Const.GameStagePreset.StageWall; break;
					case 1: p = Const.GameStagePreset.StageRiver; break;
				}

				preset[x, y] = p;
			}
		}

		// キャラクターを配置

		int ownCharacterAmount = 20;
		int oppCharacterAmount = 20;

		for (int i = 0; i < ownCharacterAmount + oppCharacterAmount; i++)
		{
			int x = 0;
			int y = 0;

			bool isOk = false;

			while(!isOk)
			{
				x = Random.Range(0, sizeX);
				y = Random.Range(0, sizeY);

				if (preset[x, y] == Const.GameStagePreset.StageGround)
				{
					isOk = true;
				}
			}

			var p = i < ownCharacterAmount ?
				Const.GameStagePreset.CharacterOwn : Const.GameStagePreset.CharacterOpp;

			if (i == 0)
			{
				p = Const.GameStagePreset.CharacterPlayer;
			}

			preset[x, y] = p;

		}

		// 食べ物を配置する
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				if (preset[x, y] != Const.GameStagePreset.StageGround)
				{
					continue;
				}

				int r = Random.Range(0, 15);

				var p = preset[x, y];

				switch(r)
				{
					case 0:
					case 1:
					case 2:
						p = Const.GameStagePreset.FoodNormal;
						break;
					case 3: p = Const.GameStagePreset.FoodSuper; break;
					case 4: p = Const.GameStagePreset.FoodEgg; break;
				}

				preset[x, y] = p;
			}
		}


		return preset;
	}
}
