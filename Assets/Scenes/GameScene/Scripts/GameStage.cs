using UnityEngine;
using System.Collections.Generic;

public class GameStage : MonoBehaviour
{
	[SerializeField]
	private int _sizeX;

	public int sizeX { get { return _sizeX; } }
	
	[SerializeField]
	private int _sizeY;

	public int sizeY { get { return _sizeY; } }


	[SerializeField]
	private Vector2 _cellSize;

	private Vector2 cellSize { get { return _cellSize; } }


	public GameMapCell[,] map { get; private set ; }

	void Awake()
	{
		InitMap();
	}

	void InitMap()
	{
		var generator = GetComponent<GameMapGeneratorBase>();
		map = generator.Generate(_sizeX, _sizeY);

		for (int x = 0; x < _sizeX; x++)
		{
			for (int y = 0; y < _sizeY; y++)
			{
				Debug.Log(map[x,y]);
			}
		}
	}
}
