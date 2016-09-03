using UnityEngine;
using UnityEngine.UI;
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

	[SerializeField]
	private ImageNumber _timeNumber;

	[SerializeField]
	private ImageNumber _ownScoreNumber;

	[SerializeField]
	private ImageNumber _oppScoreNumber;

	[SerializeField]
	private GameObject _resultRoot;

	[SerializeField]
	private GameObject _winTextRoot;

	[SerializeField]
	private GameObject _loseTextRoot;

	[SerializeField]
	private GameObject _drawTextRoot;

	[SerializeField]
	private GameObject _selectNewCharacterRoot;

	private GameCharacter _playerCharacter;

	private int _selectingOwnCharacterIndex;

	public List<GameCharacter> ownCharacters { get; private set; }
	
	public List<GameCharacter> oppCharacters { get; private set; }

	public List<GameFood> foods { get; private set; }

	public List<GameEgg> eggs { get; private set ; }

	private Const.Mode _mode;

	private FoodGenerateLogicBase _foodGenerateLogic;

	public float gameTime { get; private set; }

	public bool isGameOver { get; private set; }

	void Awake()
	{	
		if (instance != null)
		{	
			DestroyImmediate(this.gameObject);
			return;
		}

		gameTime = Setting.gameTime;

		_foodGenerateLogic = gameObject.AddComponent(Setting.foodGenerateLogicType) as FoodGenerateLogicBase;

		instance = this;

		ownCharacters = new List<GameCharacter>();
		oppCharacters = new List<GameCharacter>();

		InitMap();
		InitPlayerCharacter();
		InitOtherCharacter();
		InitFoods();

		_mode= Const.Mode.Play;
	}

	void Update()
	{
		if (isGameOver)
		{

			if (Input.GetKeyDown(KeyCode.Space))
			{
				UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
			}
			return;
		}

		switch (_mode)
		{
			case Const.Mode.Play:
				ProcessPlay();
				break;

			case Const.Mode.CharacterSelect:
				ProcessSelectPlayerCharacter();
				break;
		}

		gameTime -= Time.deltaTime;

		var gt = (int)gameTime;

		if (gt >=0)
		{
			_timeNumber.number = gt;
		}

		_ownScoreNumber.number = ownCharacters.Count;
		_oppScoreNumber.number = oppCharacters.Count;

		if (gameTime <= 0)
		{
			OnEndGameTime();
		}
		
	}

	private void OnEndGameTime()
	{
		if (isGameOver) return;
		
		isGameOver = true;

		_resultRoot.SetActive(true);

		if (ownCharacters.Count == oppCharacters.Count)
		{
			_drawTextRoot.SetActive(true);
		}
		else if (ownCharacters.Count > oppCharacters.Count)
		{
			_winTextRoot.SetActive(true);
		}
		else
		{
			_loseTextRoot.SetActive(true);
		}
	}

	private void ProcessPlay()
	{
		if (_playerCharacter != null)
		{
			_cameraRoot.position = _playerCharacter.transform.position;
		}
	}

	private void ProcessSelectPlayerCharacter()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			_selectingOwnCharacterIndex--;

			if (_selectingOwnCharacterIndex < 0)
			{
				_selectingOwnCharacterIndex = ownCharacters.Count - 1;
			}
		}

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			_selectingOwnCharacterIndex++;
			if (ownCharacters.Count <= _selectingOwnCharacterIndex)
			{
				_selectingOwnCharacterIndex = 0;
			}
		}

		var c = ownCharacters[_selectingOwnCharacterIndex];

		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetUpPlayerCharacter(c);
			_mode = Const.Mode.Play;
			Time.timeScale = 1f;
			_selectNewCharacterRoot.SetActive(false);
		}

		

		_cameraRoot.transform.position = c.transform.position;
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
					case Const.MapCellType.None: prefab = _groundPrefab; break;
					case Const.MapCellType.Wall: prefab = _wallPrefab; break;
					case Const.MapCellType.River: prefab = _riverPrefab; break;
				}

				var go = (GameObject)Instantiate(prefab);

				var pos = new Vector3(Const.cellSizeX * x, Const.cellSizeY * y, Const.mapPositionZ);

				go.transform.position = pos;
				
				go.transform.parent = this.transform;
			}
		}

		// 外の壁を作成
		for (int x = -1; x < _mapSizeX + 1; x++)
		{
			for (int y = -1; y < _mapSizeY + 1; y++)
			{
				if (0 <= x && x < _mapSizeX && 0 <= y && y < _mapSizeY) continue;

				var wall = Instantiate<GameObject>(_wallPrefab);
				var pos = new Vector3(Const.cellSizeX * x, Const.cellSizeY * y, Const.mapPositionZ);
				
				wall.transform.position = pos;
			}
		}
	}

	/// <summary>
	/// プレイヤーキャラクターを生成する
	/// </summary>
	private void InitPlayerCharacter()
	{
		SetUpPlayerCharacter(GenerateCharacter(0, 0, Const.Side.Own));
	}

	private void SetUpPlayerCharacter(GameCharacter character)
	{
		_playerCharacter = character;
		_playerCharacter.gameObject.AddComponent<GameCharacterController>();

		var aiList = _playerCharacter.GetComponents<GameCharacterAIBase>();

		foreach (var ai in aiList)
		{
			Destroy(ai);
		}

		_playerCharacter.onDead += OnDeadPlayerCharacter;
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

	private void InitFoods()
	{
		_foodGenerateLogic.OnInit();
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
		return GenerateCharacter(Const.cellSizeX * x, Const.cellSizeY + y, side);
	}
	
	public GameCharacter GenerateCharacter(float x, float y, Const.Side side)
	{
		var prefab = side == Const.Side.Own ? _characterOwnPrefab : _characterOppPrefab;

		var go = (GameObject)Instantiate(prefab);
		var character = go.GetComponent<GameCharacter>();

		character.transform.position = new Vector3(x, y, Const.characterPositionZ);
		
		character.SetUp(side);
		
		character.onDead += OnDeadCharacter;

		var list = side == Const.Side.Own ? ownCharacters : oppCharacters;

		list.Add(character);

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
		eggs.Add(egg);
		return egg;
	}

	/// <summary>
	/// 卵が孵化した際に呼ばれる
	/// </summary>
	/// <param name="egg"></param>
	public void OnHatchEgg(GameEgg egg)
	{
		var character = GenerateCharacter(egg.transform.position.x, egg.transform.position.y, egg.side);
		eggs.Remove(egg);
		character.gameObject.AddComponent<GameCharacterAIRandom>();
	}

	public void OnEatEgg(GameEgg egg)
	{
		eggs.Remove(egg);
	}

	/// <summary>
	/// 食べ物を配置する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void GenerateFood(int x, int y, Const.FoodType foodType)
	{
		var go = (GameObject)Instantiate(_foodPrefab);

		go.transform.position = new Vector3(Const.cellSizeX * x, Const.cellSizeZ * y, Const.foodPositionZ);

		go.transform.parent = transform;

		var food = go.GetComponent<GameFood>();
		foods.Add(food);
		food.SetUp(foodType);
	}

	public void OnDeadCharacter(GameCharacter character)
	{
		character.onDead -= OnDeadCharacter;

		ownCharacters.Remove(character);
		oppCharacters.Remove(character);

		if (ownCharacters.Count == 0 || oppCharacters.Count == 0)
		{
			OnEndGameTime();
		}
	}

	public void OnEatFood(GameFood food)
	{
		foods.Remove(food);
	}

	public void OnDeadPlayerCharacter(GameCharacter character)
	{
		character.onDead -= OnDeadPlayerCharacter;

		var ctrl = _playerCharacter.GetComponent<GameCharacterController>();
		Destroy(ctrl);

		_playerCharacter = null;
		
		Invoke("StartSelectPlayerCharacter", 1f);
	}

	private void StartSelectPlayerCharacter()
	{
		SelectPlayerCharacter();
	}

	public void SelectPlayerCharacter()
	{
		Time.timeScale = 0f;

		_selectingOwnCharacterIndex = 0;
		_mode = Const.Mode.CharacterSelect;
		_selectNewCharacterRoot.SetActive(true);
	}

}
