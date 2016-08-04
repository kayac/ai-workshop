﻿[System.Serializable]
public class GameMapCell
{
	public enum CellType
	{
		None,
		Wall,
		River
	}

	public int x { get; private set; }
	public int y { get; private set; }
	public CellType contentType { get; private set; }

	public GameMapCell(int x, int y, CellType contentTyoe)
	{
		this.x = x;
		this.y = y;
		this.contentType = contentTyoe;
	}

	public override string ToString()
	{
		return string.Format("x={0} y={1} : {2}", x, y, contentType);
	}
}