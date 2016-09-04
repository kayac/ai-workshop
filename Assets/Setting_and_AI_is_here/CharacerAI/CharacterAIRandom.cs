using UnityEngine;
using System.Collections;

/// <summary>
/// ランダムに移動するキャラクターAI
/// </summary>
public class GameCharacterAIRandom : CharacterAIBase
{
	void Start()
	{
		StartProcess();
	}

	[ContextMenu("process")]
	public void StartProcess()
	{
		StartCoroutine(CoRandom());
	}

	private IEnumerator CoRandom()
	{
		yield return new WaitForSeconds(2);

		while (enabled)
		{
			var isComplete = false;

			// 方向を乱数で決める
			var random = Random.Range(0, 4);
			
			var position = transform.position;

			switch (random)
			{
				case 0: position += Vector3.up; break;
				case 1: position += Vector3.down; break;
				case 2: position += Vector3.left; break;
				case 3: position += Vector3.right; break;
			}

			/// キャラクターを移動させる
			_character.MoveTo(position, () =>
			{
				isComplete = true;
			});

			while(!isComplete)
			{
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1);
		}
	}
}