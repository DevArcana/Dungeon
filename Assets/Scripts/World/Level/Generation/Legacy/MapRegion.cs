using System;
using System.Collections.Generic;
using World.Common;

namespace World.Level.Generation.Legacy
{
  public class MapRegion : IComparable<MapRegion>
  {
    public readonly List<GridPos> outline = new List<GridPos>();
    public readonly List<GridPos> cells = new List<GridPos>();
    public readonly List<MapRegion> connectedRegions = new List<MapRegion>();

    private bool _isConnectedToRoot;

    public bool IsConnectedToRoot => _isConnectedToRoot;

    public void ConnectToRoot()
    {
      _isConnectedToRoot = true;

      foreach (var region in connectedRegions)
      {
        if (!region._isConnectedToRoot)
        {
          region.ConnectToRoot();
        }
      }
    }

    public int Size => cells.Count;

    public MapRegion(GridPos start, int[,] map, int[,] mask, int tile)
    {
      var width = map.GetLength(0);
      var height = map.GetLength(1);

      var queue = new Queue<GridPos>();
      queue.Enqueue(start);

      bool IsDifferentTile(GridPos pos)
      {
        var x = pos.x;
        var y = pos.y;
        return x < 0 || y < 0 || x >= width || y >= height || map[x, y] != tile;
      }

      while (queue.Count > 0)
      {
        var cell = queue.Dequeue();
        var x = cell.x;
        var y = cell.y;

        if (mask[x, y] == 0 && !IsDifferentTile(cell))
        {
          mask[x, y] = 1;

          var left = GridPos.At(x - 1, y);
          var right = GridPos.At(x + 1, y);
          var up = GridPos.At(x, y + 1);
          var down = GridPos.At(x, y - 1);
          
          queue.Enqueue(left);
          queue.Enqueue(up);
          queue.Enqueue(right);
          queue.Enqueue(down);

          cells.Add(cell);
          if (IsDifferentTile(left) || IsDifferentTile(right) || IsDifferentTile(up) || IsDifferentTile(down))
          {
            outline.Add(cell);
          }
        }
      }
    }

    public int CompareTo(MapRegion other)
    {
      return Size.CompareTo(other.Size);
    }
  }
}