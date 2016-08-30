using System;
using System.Collections.Generic;

public static class Setting
{
	public static readonly List<CharacterLevelData> characterLevels = new List<CharacterLevelData>(
		new CharacterLevelData[] {
			
			new CharacterLevelData(level: 1, exp: 0, layEggCount: -1),
			new CharacterLevelData(level: 2, exp: 5, layEggCount: -1),
			new CharacterLevelData(level: 3, exp: 10, layEggCount: 10),
		}
	);

	public const float characterMaxLifeTime = 60f;

	public static readonly Type foodGenerateLogic = typeof(FoodGenerateLogicRandom);
}
