using UnityEngine;
using System.Collections;

public abstract class GameMapGeneratorBase : GameStageComponentBase
{
	public abstract void Generate(int sizeX, int sizeY);

	public void SetEmpty(int x, int y)
	{
		
	}

	public void SetWall(int x, int y)
	{
		var prefab = Resources.Load<GameObject>("Stage/Wall");
		SetObject(prefab, x, y);
	}

	public void SetRiver(int x, int y)
	{
		var prefab = Resources.Load<GameObject>("Stage/River");
		SetObject(prefab, x, y);
	}


	private void SetObject(GameObject prefab, int x, int y)
	{
		var go = Instantiate<GameObject>(prefab);
		go.transform.position = new Vector3(
			x * Const.cellSizeX,
			y * Const.cellSizeY,
			0
		);
	}
}
