using UnityEngine;
using System.Collections.Generic;

public class GameStage : MonoBehaviour
{
	public enum CellType
	{
		None,
		Wall,
		River
	}

	[System.Serializable]
	public class Cell
	{
		public int x { get; private set; }
		public int y { get; private set; }
		public CellType contentType { get; private set; }

		public Cell(int x, int y, CellType contentTyoe)
		{
			this.x = x;
			this.y = y;
			this.contentType = contentTyoe;
		}
	}

	public Dictionary<string, Cell> cells { get; private set; }

	[SerializeField]
	private int _sizeX;

	public int sizeX { get { return _sizeX; } }
	
	[SerializeField]
	private int _sizeY;

	public int sizeY { get { return _sizeY; } }


	[SerializeField]
	private Vector2 _cellSize;

	private Vector2 cellSize { get { return _cellSize; } }

	public static string GetCellKey(int x, int y, int z)
	{
		return string.Format("{0},{1},{2}", x, y, z);
	}
}
