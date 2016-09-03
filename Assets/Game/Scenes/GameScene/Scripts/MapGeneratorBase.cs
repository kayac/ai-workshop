using UnityEngine;

public abstract class MapGeneratorBase : MonoBehaviour
{
	public abstract MapCell[,] Generate (int sizeX, int sizeY);
}
