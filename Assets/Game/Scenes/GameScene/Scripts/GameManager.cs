using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set ; }

	public Const.MapCellType[,] map { get; private set ; }

	[SerializeField]
	private Transform _cameraRoot;

	[SerializeField]
	private GameObject _characterOwnPrefab;

	[SerializeField]
	private GameObject _characterOppPrefab;

	[SerializeField]
	private Egg _eggOwnPrefab;

	[SerializeField]
	private Egg _eggOppPrefab;

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

	private Character _playerCharacter;

	private int _selectingOwnCharacterIndex;

	public List<Character> ownCharacters { get; private set; }
	
	public List<Character> oppCharacters { get; private set; }

	public List<Food> foods { get; private set; }

	public List<Egg> eggs { get; private set ; }

	private Const.Mode _mode;

	private GamePresetBase _preset;

	private FoodGeneraterBase _foodGenerater;

	public float gameTime { get; private set; }

	public bool isGameOver { get; private set; }

	void Awake()
	{	
		if (instance != null)
		{	
			DestroyImmediate(this.gameObject);
			return;
		}


		instance = this;
		gameTime = Setting.gameTime;

		_preset = gameObject.AddComponent(Setting.presetType) as GamePresetBase;
		_foodGenerater = gameObject.AddComponent(Setting.foodGenerateLogicType) as FoodGeneraterBase;

		ownCharacters = new List<Character>();
		oppCharacters = new List<Character>();
		foods = new List<Food>();
		eggs = new List<Egg>();

		InitPreset();
		
		_mode= Const.Mode.Play;
	}

	void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
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

		_ownScoreNumber.number = ownCharacters.Count;
		_oppScoreNumber.number = oppCharacters.Count;
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

	private void InitPreset()
	{
		var presetData = _preset.Generate(Setting.mapSizeX, Setting.mapSizeY);
		InitMap(presetData);
		InitPresetContents(presetData);
	}

	/// <summary>
	/// マップを生成する
	/// </summary>
	private void InitMap(Const.GameStagePreset[,] presets)
	{

		map = new Const.MapCellType[Setting.mapSizeX, Setting.mapSizeY];
		
		for (int x = 0; x < Setting.mapSizeX; x++)
		{
			for (int y = 0; y < Setting.mapSizeY; y++)
			{
				var p = presets[x, y];

				var content = Const.MapCellType.None;

				switch(p)
				{
					case Const.GameStagePreset.StageRiver: content = Const.MapCellType.River; break;
					case Const.GameStagePreset.StageWall : content = Const.MapCellType.Wall; break;
				}

				map[x, y] = content;

				GameObject prefab = null;

				switch (content)
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
		for (int x = -1; x < Setting.mapSizeX + 1; x++)
		{
			for (int y = -1; y < Setting.mapSizeY + 1; y++)
			{
				if (0 <= x && x < Setting.mapSizeX && 0 <= y && y < Setting.mapSizeY) continue;

				var wall = Instantiate<GameObject>(_wallPrefab);
				var pos = new Vector3(Const.cellSizeX * x, Const.cellSizeY * y, Const.mapPositionZ);
				
				wall.transform.position = pos;
			}
		}
	}


	private void InitPresetContents(Const.GameStagePreset[,] presets)
	{
		for (int x = 0; x < Setting.mapSizeX; x++)
		{
			for (int y = 0; y < Setting.mapSizeY; y++)
			{
				switch(presets[x, y])
				{
					case Const.GameStagePreset.CharacterPlayer:
						SetUpPlayerCharacter(GenerateCharacter(x, y, Const.Side.Own));
						break;

					case Const.GameStagePreset.CharacterOwn:
						SetUpCharacterAI(GenerateCharacter(x, y, Const.Side.Own));
						break;

					case Const.GameStagePreset.CharacterOpp:
						SetUpCharacterAI(GenerateCharacter(x, y, Const.Side.Opp));
						break;

					case Const.GameStagePreset.EggOwn:
						GenerateEgg(x, y, Const.Side.Own);
						break;

					case Const.GameStagePreset.EggOpp:
						GenerateEgg(x, y, Const.Side.Opp);
						break;

					case Const.GameStagePreset.FoodNormal:
						GenerateFood(x, y, Const.FoodType.Normal);
						break;
					case Const.GameStagePreset.FoodSuper:
						GenerateFood(x, y, Const.FoodType.Super);
						break;

					case Const.GameStagePreset.FoodEgg:
						GenerateFood(x, y, Const.FoodType.Egg);
						break;
				}				
			}
		}
	}



	private void SetUpPlayerCharacter(Character character)
	{
		_playerCharacter = character;
		_playerCharacter.gameObject.AddComponent<CharacterController>();

		var aiList = _playerCharacter.GetComponents<CharacterAIBase>();

		foreach (var ai in aiList)
		{
			Destroy(ai);
		}

		_playerCharacter.onDead += OnDeadPlayerCharacter;
	}

	private void SetUpCharacterAI(Character character)
	{
		character.gameObject.AddComponent(
			character.side == Const.Side.Own ?
			Setting.ownCharacterAIType : Setting.oppCharacterAIType
		);
	}

	/// <summary>
	/// キャラクターを生成する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="side"></param>
	/// <returns></returns>
	public Character GenerateCharacter(int x, int y, Const.Side side)
	{
		return GenerateCharacter(Const.cellSizeX * x, Const.cellSizeY * y, side);
	}
	
	/// <summary>
	/// キャラクターを生成する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="side"></param>
	/// <returns></returns>
	public Character GenerateCharacter(float x, float y, Const.Side side)
	{
		var prefab = side == Const.Side.Own ? _characterOwnPrefab : _characterOppPrefab;

		var go = (GameObject)Instantiate(prefab);
		var character = go.GetComponent<Character>();

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
	public Egg GenerateEgg(int mapX, int mapY, Const.Side side)
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
	public Egg GenerateEgg(float x, float y, Const.Side side)
	{
		var prefab = side == Const.Side.Own ? _eggOwnPrefab : _eggOppPrefab;
		var egg = Instantiate<Egg>(prefab);
		egg.transform.position = new Vector3(x, y, Const.eggPositionZ);
		egg.SetUp(side);
		eggs.Add(egg);
		return egg;
	}

	/// <summary>
	/// 卵が孵化した際に呼ばれる
	/// </summary>
	/// <param name="egg"></param>
	public void OnHatchEgg(Egg egg)
	{
		var character = GenerateCharacter(egg.transform.position.x, egg.transform.position.y, egg.side);
		SetUpCharacterAI(character);
		eggs.Remove(egg);
	}

	/// <summary>
	/// 卵が孵化した
	/// </summary>
	/// <param name="egg"></param>
	public void OnEatEgg(Egg egg)
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

		var food = go.GetComponent<Food>();
		foods.Add(food);
		food.SetUp(foodType);
	}

	/// <summary>
	/// キャラクターが死亡した時
	/// </summary>
	/// <param name="character"></param>
	public void OnDeadCharacter(Character character)
	{
		character.onDead -= OnDeadCharacter;

		ownCharacters.Remove(character);
		oppCharacters.Remove(character);

		if (ownCharacters.Count == 0 || oppCharacters.Count == 0)
		{
			OnEndGameTime();
		}
	}

	/// <summary>
	/// 食べ物が食べられた時
	/// </summary>
	/// <param name="food"></param>
	public void OnEatFood(Food food)
	{
		foods.Remove(food);
	}

	/// <summary>
	/// プレイヤーが操作するキャラクターが死亡した時
	/// </summary>
	/// <param name="character"></param>
	public void OnDeadPlayerCharacter(Character character)
	{
		character.onDead -= OnDeadPlayerCharacter;

		var ctrl = _playerCharacter.GetComponent<CharacterController>();
		Destroy(ctrl);

		_playerCharacter = null;
		
		Invoke("StartSelectPlayerCharacter", 1f);
	}

	/// <summary>
	/// プレイヤーが新しく操作するキャラクターを選択する
	/// </summary>
	private void StartSelectPlayerCharacter()
	{
		SelectPlayerCharacter();
	}

	/// <summary>
	/// プレイヤーが新しく操作するキャラクターを選択する
	/// </summary>
	public void SelectPlayerCharacter()
	{
		Time.timeScale = 0f;

		_selectingOwnCharacterIndex = 0;
		_mode = Const.Mode.CharacterSelect;
		_selectNewCharacterRoot.SetActive(true);
	}

}
