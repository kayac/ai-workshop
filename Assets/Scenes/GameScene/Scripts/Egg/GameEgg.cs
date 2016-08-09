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
	/// 孵化した時に呼ばれる
	/// </summary>
	public event System.Action<GameEgg> onHatch;

	/// <summary>
	/// 既に孵化しているか
	/// </summary>
	/// <returns></returns>
	public bool alreadyHatch { get; private set; }

	void Update()
	{
		if (!alreadyHatch)
		{
			_currentTime += Time.deltaTime;

			if (hatchTime <= _currentTime)
			{
				if (onHatch != null)
				{
					onHatch(this);
				}
				alreadyHatch = true;
			}
		}
	}
}
