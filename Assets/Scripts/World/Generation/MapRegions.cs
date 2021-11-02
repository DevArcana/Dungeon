using System.Collections.Generic;
using System.Linq;
using World.Common;

namespace World.Generation
{
  public class MapRegions
  {
    private readonly SerializableMap<bool> _map;
    private readonly RegionsMap _regions;

    private readonly int _maxRegionSize;

    private int _regionIndex;

    public RegionsMap Regions => _regions;

    public MapRegions(SerializableMap<bool> map, int maxRegionSize)
    {
      _map = map;
      _maxRegionSize = maxRegionSize;
      _regions = new RegionsMap(map.width, map.height);

      ScanRegions();
      Antialias();
    }

    private void ScanRegion(int sx, int sy)
    {
      var queue = new Queue<GridPos>();
      queue.Enqueue(GridPos.At(sx, sy));

      _regionIndex++;

      var size = 0;

      while (queue.Count > 0)
      {
        var pos = queue.Dequeue();
        
        if (!_map.WithinBounds(pos) || _regions[pos] != 0)
        {
          continue;
        }
        
        if (_map[pos])
        {
          _regions[pos] = -1;
        }
        else
        {
          _regions[pos] = _regionIndex;
          size++;

          if (size > _maxRegionSize)
          {
            break;
          }
          
          queue.Enqueue(pos.North);
          queue.Enqueue(pos.East);
          queue.Enqueue(pos.South);
          queue.Enqueue(pos.West);
        }
      }
    }
    
    private void ScanRegions()
    {
      for (var y = 0; y < _map.height; y++)
      {
        for (var x = 0; x < _map.width; x++)
        {
          if (_map[x, y])
          {
            _regions[x, y] = -1;
          }
          else if (_regions[x, y] == 0)
          {
            ScanRegion(x, y);
          }
        }
      }
    }

    private int DecideRegion(int sx, int sy)
    {
      var neighbours = new Dictionary<int, int>
      {
        [_regions[sx, sy]] = 1
      };
      
      for (var y = sy - 1; y <= sy + 1; y++)
      {
        for (var x = sx - 1; x <= sx + 1; x++)
        {
          if (x == sx && y == sy)
          {
            continue;
          }

          if (!_regions.WithinBounds(x, y) || _map[x, y])
          {
            continue;
          }

          var region = _regions[x, y];
          if (neighbours.ContainsKey(region))
          {
            neighbours[region]++;
          }
          else
          {
            neighbours[region] = 1;
          }
        }
      }

      return neighbours.OrderBy(x => x.Value).FirstOrDefault().Key;
    }
    
    private void Antialias()
    {
      var regions = new SerializableMap<int>(_regions.width, _regions.height);
      for (var y = 0; y < _regions.height; y++)
      {
        for (var x = 0; x < _regions.width; x++)
        {
          if (_regions[x, y] > 0)
          {
            // count neighbours in 4 cardinal directions of the same region type
            regions[x, y] = DecideRegion(x, y);
          }
          else
          {
            regions[x, y] = -1;
          }
        }
      }

      for (var y = 0; y < _regions.height; y++)
      {
        for (var x = 0; x < _regions.width; x++)
        {
          _regions[x, y] = regions[x, y];
        }
      }
    }
  }
}