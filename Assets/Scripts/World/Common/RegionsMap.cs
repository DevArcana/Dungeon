using System;
using System.Collections.Generic;
using System.Linq;

namespace World.Common
{
  public class Region
  {
    public readonly int index;
    public readonly List<GridPos> cells;
    public int Size => cells.Count;

    public Region(int i)
    {
      index = i;
      cells = new List<GridPos>();
    }
  }
  
  [Serializable]
  public class RegionsMap : SerializableMap<int>
  {
    public RegionsMap(int width, int height) : base(width, height)
    {
    }

    public IEnumerable<Region> AllRegions()
    {
      var regions = new Dictionary<int, Region>();

      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          var i = this[x, y];

          if (i > 0)
          {
            if (!regions.ContainsKey(i))
            {
              regions[i] = new Region(i);
            }

            var region = regions[i];
            region.cells.Add(GridPos.At(x, y));
          }
        }
      }

      return regions.Values.OrderByDescending(x => x.cells.Count);
    }

    public bool Contains(int region, IEnumerable<GridPos> cells)
    {
      foreach (var cell in cells)
      {
        if (!WithinBounds(cell) || this[cell] != region)
        {
          return false;
        }
      }

      return true;
    }
    
    public Region GetRegion(int index)
    {
      var region = new Region(index);
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          var i = this[x, y];

          if (i == index)
          {
            region.cells.Add(GridPos.At(x, y));
          }
        }
      }

      return region;
    }
  }
}