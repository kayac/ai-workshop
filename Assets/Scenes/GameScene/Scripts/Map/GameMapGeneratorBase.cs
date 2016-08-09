using UnityEngine;

public abstract class GameMapGeneratorBase : MonoBehaviour
{
	public abstract GameMapCell[,] Generate (int sizeX, int sizeY);
}
