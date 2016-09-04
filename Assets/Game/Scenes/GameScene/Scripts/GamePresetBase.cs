using UnityEngine;
using System.Collections;

public abstract class GamePresetBase : MonoBehaviour
{
	public abstract Const.GameStagePreset[,] Generate(int sizeX, int sizeY);
}
