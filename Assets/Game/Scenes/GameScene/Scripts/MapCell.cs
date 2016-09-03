[System.Serializable]
public class MapCell
{
	public int x { get; private set; }
	public int y { get; private set; }
	public Const.MapCellType contentType { get; private set; }

	public MapCell(int x, int y, Const.MapCellType contentTyoe)
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