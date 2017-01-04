using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CharacterAIRule : CharacterAIBase
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

	private void Approach(Action onComplete, MonoBehaviour target)
	{
		var position = _character.transform.position;

		var destination = target.transform.position;
		var direction = destination - position;

		if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)) {
			position += (direction.x > 0 ? Vector3.right : Vector3.left);
		} else {
			position += (direction.y > 0 ? Vector3.up : Vector3.down);
		}

		_character.MoveTo(position, onComplete);
	}
	private void TryApproachFood(Action onComplete)
	{
		var target = (from f in GameManager.instance.foods
			let d = (f.transform.position - _character.transform.position).sqrMagnitude
			where d < 16
			orderby d ascending
			select f).FirstOrDefault();
		if (target != null)
			Approach(onComplete, target);
		else
			RandomWalk(onComplete);
	}
	private void TryApproachEnemy(Action onComplete)
	{
		var target = (from c in GameManager.instance.ownCharacters
			where (c.level < _character.level) || (!c.isSuperMode && _character.isSuperMode)
			let d = (c.transform.position - _character.transform.position).sqrMagnitude
			where d < 16
			orderby d ascending
			select c).FirstOrDefault();
		if (target != null)
			Approach(onComplete, target);
		else
			RandomWalk(onComplete);
	}
	private void RandomWalk(Action onComplete)
	{
		var position = transform.position;

		var random = UnityEngine.Random.Range(0, 4);
		switch (random)
		{
		case 0: position += Vector3.up; break;
		case 1: position += Vector3.down; break;
		case 2: position += Vector3.left; break;
		case 3: position += Vector3.right; break;
		}

		_character.MoveTo(position, onComplete);
	}

	private IEnumerator CoRandom()
	{
		yield return new WaitForSeconds(2);

		while (enabled)
		{
			var isComplete = false;
			Action<Action> action;

			if (_character.level <= 1)
				action = TryApproachFood;
			else
				action = TryApproachEnemy;

			action(() => { isComplete = true; });

			while(!isComplete)
			{
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1);
		}
	}
}
