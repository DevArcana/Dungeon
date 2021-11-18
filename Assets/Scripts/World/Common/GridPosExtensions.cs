using System.Collections.Generic;
using System.Linq;

namespace World.Common
{
  public static class GridPosExtensions
  {
    public static IEnumerable<GridPos> Walkable(this IEnumerable<GridPos> cells)
    {
      var world = World.instance;
      return cells.Where(x => world.IsWalkable(x));
    }
  }
}