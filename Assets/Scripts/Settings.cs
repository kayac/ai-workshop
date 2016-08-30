using System;
using System.Collections.Generic;

/// <summary>
/// ゲームのバランス調整を行う設定クラス
/// </summary>
public static class Setting
{
	public static readonly List<CharacterLevelData> characterLevels = new List<CharacterLevelData>(
		new CharacterLevelData[] {
			
			new CharacterLevelData(level: 1, exp: 0, layEggCount: -1),
			new CharacterLevelData(level: 2, exp: 5, layEggCount: -1),
			new CharacterLevelData(level: 3, exp: 10, layEggCount: 10),
		}
	);

	/// <summary>
	/// キャラクターの最大寿命(秒)
	/// </summary>
	public const float characterMaxLifeTime = 60f;

	/// <summary>
	/// フードを生成するロジックを指定する
	/// </summary>
	/// <param name="FoodGenerateLogicRandom"></param>
	/// <returns></returns>
	public static readonly Type foodGenerateLogicType = typeof(
		// ここのクラスを変更するとゲーム開始時に使用するロジックが適用される
		FoodGenerateLogicRandom
	);
}
