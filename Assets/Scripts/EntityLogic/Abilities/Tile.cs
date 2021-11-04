using World.Common;

namespace EntityLogic.Abilities
{
  public class Tile
  {
    
    public GridPos gridPos;
    public byte height;
    public int cost;

    public Tile(GridPos gridPos, byte height, int cost)
    {
      this.gridPos = gridPos;
      this.height = height;
      this.cost = cost;
    }
  }
}