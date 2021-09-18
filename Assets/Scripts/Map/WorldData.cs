using Grid;
using TurnSystem;

namespace Map
{
  public class WorldData
  {
    public int x;
    public int y;

    public int height;

    public GridEntity occupant;

    public WorldData(int x, int y, int height)
    {
      this.x = x;
      this.y = y;
      this.height = height;
      this.occupant = null;
    }
  }
}