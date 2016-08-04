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

	[SerializeField]
	private GameObject _groundPrefab;

	[SerializeField]
	private GameObject _wallPrefab;

	[SerializeField]
	private GameObject _riverPrefab;

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
				var cell = map[x, y];
				
				GameObject prefab = null;

				switch (cell.contentType)
				{
					case GameMapCell.CellType.None: prefab = _groundPrefab; break;
					case GameMapCell.CellType.Wall: prefab = _wallPrefab; break;
					case GameMapCell.CellType.River: prefab = _riverPrefab; break;
				}

				var go = (GameObject)Instantiate(prefab);

				go.transform.position = new Vector3(
					Const.cellSizeX * x, Const.cellSizeY * y, 0
				);

				go.transform.parent = this.transform;
			}
		}

		
	}
}
