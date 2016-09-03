using UnityEngine;

public abstract class GameMapGeneratorBase : MonoBehaviour
{
	public abstract MapCell[,] Generate (int sizeX, int sizeY);
}
