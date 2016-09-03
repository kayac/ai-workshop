using UnityEngine;
using System.Collections;

public abstract class GamePresetBase : MonoBehaviour
{
	//public abstract MapCell[,] Generate(int sizeX, int sizeY);

	public abstract Const.GameStagePreset[,] Generate(int sizeX, int sizeY);
}
