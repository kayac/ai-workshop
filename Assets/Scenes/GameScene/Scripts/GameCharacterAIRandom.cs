using UnityEngine;
using System.Collections;

public class GameCharacterAIRandom : GameCharacterAIBase
{
	void Start()
	{
		//StartProcess();
	}

	[ContextMenu("process")]
	public void StartProcess()
	{
		StartCoroutine(CoRandom());
	}

	private IEnumerator CoRandom()
	{
		var count = 0;
		while (count < 3)
		{
			var isComplete = false;

			var random = Random.Range(0, 4);

			var position = transform.position;

			switch (random)
			{
				case 0: position += Vector3.up; break;
				case 1: position += Vector3.down; break;
				case 2: position += Vector3.left; break;
				case 3: position += Vector3.right; break;
			}

			_character.MoveTo(position, () =>
			{
				isComplete = true;
			});

			while(!isComplete)
			{
				yield return new WaitForEndOfFrame();
			}
			count++;
		}
	}
}