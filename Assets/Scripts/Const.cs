using UnityEngine;

public static class Const
{
	public const float cellSizeX = 1f;
	public const float cellSizeY = 1f;
	public const float cellSizeZ = 1f;
	
	public static readonly Vector2 cellSizeVector2 = new Vector2(cellSizeX, cellSizeY);

	public const float mapPositionZ = 0.3f;
	public const float characterPositionZ = 0f;
	public const float foodPositionZ = 0.2f;
	public const float eggPositionZ = 0.1f;

	public enum Side
	{
		Own,
		Opp
	}

	public enum Mode
	{
		Play,
		CharacterSelect
	}

	public enum MapCellType
	{
		None,
		Wall,
		River
	}

	public enum FoodType
	{
		Normal,
		Super,
		Egg
	}

	public const string layerNameCharacter	= "Character";
	public const string layerNameFood 		= "Food";
	public const string layerNameEgg		= "Egg";
}
