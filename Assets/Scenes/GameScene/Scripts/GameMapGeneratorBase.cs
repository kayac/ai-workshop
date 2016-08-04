public abstract class GameMapGeneratorBase : GameStageComponentBase
{
	public abstract GameMapCell[,] Generate (int sizeX, int sizeY);
}
