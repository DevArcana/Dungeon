using Grid;

namespace Map.Legacy
{
  public class WorldData
  {
    public int x;
    public int y;

    public int height;

    public GridEntity occupant;

    public int regionIndex = -1;

    public WorldData(int x, int y, int height)
    {
      this.x = x;
      this.y = y;
      this.height = height;
      this.occupant = null;
    }
  }
}