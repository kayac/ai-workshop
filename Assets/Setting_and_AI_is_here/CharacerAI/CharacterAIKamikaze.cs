using UnityEngine;
using System.Collections;

public class GameCharacterAIKamikaze : CharacterAIBase
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
			var player = GameManager.instance.playerCharacter;

			if (player != null)
			{
				var position = transform.position;
				var path = GameManager.instance.navigator.FindPath(position, player.transform.position);

				if (path != null)
				{
					position += path[0];

					var isComplete = false;
					_character.MoveTo(position, () => {
						isComplete = true;
					});
					while (!isComplete)
						yield return new WaitForEndOfFrame();
				}
				else
					Debug.Log("No Path");
			}
			else
				Debug.Log("No Player");

			yield return new WaitForSeconds(1);
		}
	}
}