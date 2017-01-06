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

	private bool IsWall(Vector3 position)
	{
		var x = (int) position.x;
		var y = (int) position.y;

		if (x < 0 || x >= GameManager.instance.map.GetLength(0)
		    || y < 0 || y >= GameManager.instance.map.GetLength(1))
			return true;

		return GameManager.instance.map[x, y] != Const.MapCellType.None;
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
	private void Avoid(Action onComplete, MonoBehaviour target)
	{
		var position = _character.transform.position;

		var destination = target.transform.position;
		var direction = destination - position;

		Vector3 move;
		if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)) {
			move = (direction.x > 0 ? Vector3.left : Vector3.right);
			if (IsWall(position + move))
				move = (IsWall(position + Vector3.up) ? Vector3.down :  Vector3.up);
		} else {
			move = (direction.y > 0 ? Vector3.down : Vector3.up);
			if (IsWall(position + move))
				move = (IsWall(position + Vector3.left) ? Vector3.right :  Vector3.left);
		}

		_character.MoveTo(position + move, onComplete);
	}

	private bool IsEatable(Character target)
	{
		return !target.isSuperMode && (_character.isSuperMode || target.level < _character.level);
	}
	private bool IsEatableBy(Character target)
	{
		return !_character.isSuperMode && (target.isSuperMode || _character.level < target.level);
	}

	private void RandomWalk(Action onComplete, MonoBehaviour target)
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

	private MonoBehaviour FindFood()
	{
		// レベルが上がっていれば食べない
		if (_character.level >= 2)
			return null;

		// 近くの食べ物を探す
		return (from f in GameManager.instance.foods
			let d = (f.transform.position - _character.transform.position).sqrMagnitude
			where d < 16
			orderby d ascending
			select f).FirstOrDefault();
	}

	private MonoBehaviour FindEatableEnemy()
	{
		var target = GameManager.instance.playerCharacter;

		if (target != null && IsEatable(target) && (target.transform.position - _character.transform.position).sqrMagnitude < 16)
			return target;

		return null;
	}

	private MonoBehaviour FindStrongerEnemy()
	{
		var target = GameManager.instance.playerCharacter;

		if (target != null && IsEatableBy(target) && (target.transform.position - _character.transform.position).sqrMagnitude < 16)
			return target;

		return null;
	}

	private IEnumerator CoRandom()
	{
		yield return new WaitForSeconds(2);

		while (enabled)
		{
			var isComplete = false;
			MonoBehaviour target;
			Action<Action, MonoBehaviour> action;

			if ((target = FindStrongerEnemy()) != null)
				action = Avoid;
			else if ((target = FindFood()) != null)
				action = Approach;
			else if ((target = FindEatableEnemy()) != null)
				action = Approach;
			else
				action = RandomWalk;

			action(() => { isComplete = true; }, target);

			while(!isComplete)
			{
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1);
		}
	}
}
