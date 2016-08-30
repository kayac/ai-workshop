﻿[System.Serializable]
public class GameMapCell
{
	public int x { get; private set; }
	public int y { get; private set; }
	public Const.MapCellType contentType { get; private set; }

	public bool hasFood { get; set; } 

	public GameMapCell(int x, int y, Const.MapCellType contentTyoe, bool hasFood)
	{
		this.x = x;
		this.y = y;
		this.contentType = contentTyoe;
		this.hasFood = hasFood;
	}

	public override string ToString()
	{
		return string.Format("x={0} y={1} : {2}", x, y, contentType);
	}
}