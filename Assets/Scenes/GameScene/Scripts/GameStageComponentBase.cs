using UnityEngine;
using System.Collections;

public abstract class GameStageComponentBase : MonoBehaviour
{

	private GameStage _stageInternal;

	protected GameStage _stage
	{
		get
		{
			if (_stageInternal == null)
			{
				_stageInternal = GetComponent<GameStage>();
			}
			return _stageInternal;
		}
	}
}
