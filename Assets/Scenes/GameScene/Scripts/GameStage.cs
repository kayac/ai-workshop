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
	private Transform _cameraRoot;

	[SerializeField]
	private GameObject _characterOwnPrefab;

	[SerializeField]
	private GameObject _characterOppPrefab;

	[SerializeField]
	private GameObject _groundPrefab;

	[SerializeField]
	private GameObject _wallPrefab;

	[SerializeField]
	private GameObject _riverPrefab;

	[SerializeField]
	private GameObject _foodPrefab;

	private GameCharacter _playerCharacter;

	void Awake()
	{
		InitMap();
		InitPlayerCharacter();
	}

	void Update()
	{
		if (_playerCharacter != null)
		{
			_cameraRoot.position = _playerCharacter.transform.position;
		}
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

				var pos = new Vector3(Const.cellSizeX * x, Const.cellSizeY * y, Const.mapPositionZ);

				go.transform.position = pos;
				
				go.transform.parent = this.transform;

				if (cell.hasFood)
				{
					SetFood(x, y);
				}
			}
		}

	}

	void InitPlayerCharacter()
	{
		_playerCharacter = GeneratePlayer(0, 0, Const.Side.Own);
		_playerCharacter.gameObject.AddComponent<GameCharacterController>();
	}

	public GameCharacter GeneratePlayer(int x, int y, Const.Side side)
	{
		var prefab = side == Const.Side.Own ? _characterOwnPrefab : _characterOppPrefab;

		var go = (GameObject)Instantiate(prefab);
		var character = go.GetComponent<GameCharacter>();

		character.transform.position = new Vector3(
			Const.cellSizeX * x, Const.cellSizeY * y, Const.characterPositionZ);
		
		character.SetUp(side);

		return character;
	}

	public void SetFood(int x, int y)
	{
		var go = (GameObject)Instantiate(_foodPrefab);

		go.transform.position = new Vector3(Const.cellSizeX * x, Const.cellSizeZ * y, Const.foodPositionZ);

		go.transform.parent = transform;
	}
}
