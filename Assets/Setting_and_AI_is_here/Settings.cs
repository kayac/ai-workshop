﻿using System;
using System.Collections.Generic;

/// <summary>
/// ゲームのバランス調整を行う設定クラス
/// </summary>
public static class Setting
{
	public static readonly Type presetType = typeof(
		// ここのクラスを変更するとゲーム開始時のステージを設定できる
		GamePresetDesigned
	);

	public static readonly Type ownCharacterAIType = typeof(
		// ここのクラスを変更すると自チームのキャラのAIを設定できる
		GameCharacterAIRandom
	);

	public static readonly Type oppCharacterAIType = typeof(
		// ここのクラスを変更すると敵チームのキャラのAIを設定できる
		GameCharacterAIRandom
	);

	public static readonly Type foodGenerateLogicType = typeof(
		// ここのクラスを変更するとゲーム開始時に使用するロジックが適用される
		FoodGeneraterRandom
	);


	public static readonly List<CharacterLevelData> characterLevels = new List<CharacterLevelData>(
		new CharacterLevelData[] {
			
			new CharacterLevelData(level: 1, exp: 0, layEggCount: -1, speed: 3),
			new CharacterLevelData(level: 2, exp: 5, layEggCount: -1, speed: 3),
			new CharacterLevelData(level: 3, exp: 10, layEggCount: 10, speed: 3),
		}
	);

	public const int mapSizeX = 20;
	public const int mapSizeY = 20;

	/// <summary>
	/// 制限時間(秒)
	/// </summary>
	public const float gameTime = 100;

	/// <summary>
	/// キャラクターに寿命をもたせるか
	/// </summary>
	public static bool isEnableLifeTime = false;

	/// <summary>
	/// キャラクターの最大寿命(秒)
	/// </summary>
	public const float characterMaxLifeTime = 60f;

	

	/// <summary>
	/// 食べ物を食べた時に貰える経験値
	/// </summary>
	public const int expEatFood = 1;

	/// <summary>
	/// 卵を食べた時に貰える経験値
	/// </summary>
	public const int expEatEgg = 2;

	/// <summary>
	/// 敵を食べた時に貰える経験値
	/// </summary>
	public const int expEatCharacter = 5;

	/// <summary>
	/// 食べ物を食べた時に貰える産卵カウント
	/// </summary>
	public const int layEggCountEatFood = 1;

	/// <summary>
	/// 卵を食べた時に貰える産卵カウント
	/// </summary>
	public const int layEggCountEatEgg = 2;

	/// <summary>
	/// キャラクターを食べた時に貰える産卵カウント
	/// </summary>
	public const int layEggCountEatCharacter = 3;

	/// <summary>
	/// スーパー状態の持続時間(秒)
	/// </summary>
	public const float superModeTime = 10;

	/// <summary>
	/// 味方が繁殖しないモード
	/// </summary>
	public const bool noEggLayMode = true;
}
