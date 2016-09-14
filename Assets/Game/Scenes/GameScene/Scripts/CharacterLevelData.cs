public class CharacterLevelData
{
	
	private int _level;
	
	/// <summary>
	/// レベル
	/// </summary>
	/// <returns></returns>
	public int level { get { return _level; } }

	private int _exp;

	/// <summary>
	/// このレベルに到達するまでに必要な経験値
	/// </summary>
	/// <returns></returns>
	public int exp { get { return _exp; } }

	private int _layEggCount;

	/// <summary>
	/// 卵を産む回数
	/// </summary>
	/// <returns></returns>
	public int layEggCount { get { return _layEggCount; } }

	private float _speed = 3f;

	/// <summary>
	/// 1秒あたりの移動速度
	/// </summary>
	/// <returns></returns>
	public float speed { get { return _speed; } }

	/// <summary>
	/// レベルのマスターデーター
	/// </summary>
	/// <param name="level">レベル</param>
	/// <param name="exp">このレベルに到達するまでに必要な経験値</param>
	/// <param name="layEggCount">卵を産む回数</param>
	public CharacterLevelData(int level, int exp, int layEggCount, float speed)
	{
		_level = level;
		_exp = exp;
		_layEggCount = layEggCount;
		_speed = speed;
	}
}