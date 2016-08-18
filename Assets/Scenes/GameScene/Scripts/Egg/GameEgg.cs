using UnityEngine;
using System.Collections;

/// <summary>
/// 卵 : 一定時間で孵化する
/// </summary>
public class GameEgg : MonoBehaviour
{
	/// <summary>
	/// 産まれるまでの時間(秒)
	/// </summary>
	public float hatchTime = 10f;

	/// <summary>
	/// 現在経過している時間(秒)
	/// </summary>
	private float _currentTime;

	/// <summary>
	/// 既に孵化しているか
	/// </summary>
	/// <returns></returns>
	public bool alreadyHatch { get; private set; }

	/// <summary>
	/// 味方の卵/敵の卵
	/// </summary>
	/// <returns></returns>
	public Const.Side side { get; private set; }

	public void SetUp(Const.Side side)
	{
		this.side = side;
	}

	void Update()
	{
		if (!alreadyHatch)
		{
			_currentTime += Time.deltaTime;

			if (hatchTime <= _currentTime)
			{
				GameManager.instance.GenerateEgg(transform.position.x, transform.position.y, side);
				alreadyHatch = true;
				OnHatch();
			}
		}
	}

	public void OnEat()
	{
		Destroy(this.gameObject);
	}

	private void OnHatch()
	{
		Destroy(this.gameObject);
	}
}
