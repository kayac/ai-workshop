using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set ; }

	[SerializeField]
	private int _mapSizeX = 10;

	public int mapSizeX { get { return _mapSizeX; } }
	
	[SerializeField]
	private int _mapSizeY = 10;

	public int mapSizeY { get { return _mapSizeY; } }

	public GameMapCell[,] map { get; private set ; }

	[SerializeField]
	private Transform _cameraRoot;

	[SerializeField]
	private GameObject _characterOwnPrefab;

	[SerializeField]
	private GameObject _characterOppPrefab;

	[SerializeField]
	private GameEgg _eggOwnPrefab;

	[SerializeField]
	private GameEgg _eggOppPrefab;

	[SerializeField]
	private GameObject _groundPrefab;

	[SerializeField]
	private GameObject _wallPrefab;

	[SerializeField]
	private GameObject _riverPrefab;

	[SerializeField]
	private GameObject _foodPrefab;

	private GameCharacter _playerCharacter;

	public List<GameCharacter> ownPlayers { get; private set; }
	
	public List<GameCharacter> oppPlayers { get; private set; }

	public List<GameFood> foods { get; private set; }

	public List<GameEgg> eggs { get; private set ; }

	void Awake()
	{
		if (instance != null)
		{	
			DestroyImmediate(this.gameObject);
			return;
		}

		instance = this;

		InitMap();
		InitPlayerCharacter();
		InitOtherCharacter();
	}

	void Update()
	{
		if (_playerCharacter != null)
		{
			_cameraRoot.position = _playerCharacter.transform.position;
		}
	}

	/// <summary>
	/// マップを生成する
	/// </summary>
	private void InitMap()
	{
		var generator = GetComponent<GameMapGeneratorBase>();
		map = generator.Generate(_mapSizeX, _mapSizeY);

		for (int x = 0; x < _mapSizeX; x++)
		{
			for (int y = 0; y < _mapSizeY; y++)
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

	/// <summary>
	/// プレイヤーキャラクターを生成する
	/// </summary>
	private void InitPlayerCharacter()
	{
		_playerCharacter = GenerateCharacter(0, 0, Const.Side.Own);
		_playerCharacter.gameObject.AddComponent<GameCharacterController>();
	}

	/// <summary>
	/// 他のキャラを生成する
	/// </summary>
	void InitOtherCharacter()
	{
		int count = 20;

		for(int i = 0; i < count; i++)
		{
			var side = count / 2 < i ? Const.Side.Own : Const.Side.Opp;

			var x = Random.Range(0, _mapSizeX);
			var y = Random.Range(0, _mapSizeY);

			var character = GenerateCharacter(x, y, side);

			character.gameObject.AddComponent<GameCharacterAIRandom>();
		}
	}

	/// <summary>
	/// キャラクターを生成する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="side"></param>
	/// <returns></returns>
	public GameCharacter GenerateCharacter(int x, int y, Const.Side side)
	{
		var prefab = side == Const.Side.Own ? _characterOwnPrefab : _characterOppPrefab;

		var go = (GameObject)Instantiate(prefab);
		var character = go.GetComponent<GameCharacter>();

		character.transform.position = new Vector3(
			Const.cellSizeX * x, Const.cellSizeY * y, Const.characterPositionZ);
		
		character.SetUp(side);
		return character;
	}

	/// <summary>
	/// 卵を生成する(マップのグリッドに沿って)
	/// </summary>
	/// <param name="mapX"></param>
	/// <param name="mapY"></param>
	/// <param name="side"></param>
	/// <returns></returns>
	public GameEgg GenerateEgg(int mapX, int mapY, Const.Side side)
	{
		var x = Const.cellSizeX * mapX;
		var y = Const.cellSizeY * mapY;

		return GenerateEgg(x, y, side);
	}

	/// <summary>
	/// 卵を生成する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="side"></param>
	/// <returns></returns>
	public GameEgg GenerateEgg(float x, float y, Const.Side side)
	{
		var prefab = side == Const.Side.Own ? _eggOwnPrefab : _eggOppPrefab;
		var egg = Instantiate<GameEgg>(prefab);
		egg.transform.position = new Vector3(x, y, Const.eggPositionZ);
		egg.SetUp(side);
		return egg;
	}

	/// <summary>
	/// 卵が孵化した際に呼ばれる
	/// </summary>
	/// <param name="egg"></param>
	private void OnHatchEgg(GameEgg egg)
	{
		var character = GenerateCharacter((int)egg.transform.position.x, (int)egg.transform.position.y, egg.side);
		character.gameObject.AddComponent<GameCharacterAIRandom>();
	}

	/// <summary>
	/// 食べ物を配置する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void SetFood(int x, int y)
	{
		var go = (GameObject)Instantiate(_foodPrefab);

		go.transform.position = new Vector3(Const.cellSizeX * x, Const.cellSizeZ * y, Const.foodPositionZ);

		go.transform.parent = transform;
	}

	public void OnDeadPlayerCharacter()
	{
		
	}
}
