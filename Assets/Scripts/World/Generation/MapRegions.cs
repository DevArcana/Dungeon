using System.Collections.Generic;
using World.Common;

namespace World.Generation
{
  public class MapRegions
  {
    private readonly SerializableMap<bool> _map;
    private readonly SerializableMap<int> _regions;

    private readonly int _maxRegionSize;

    private int _regionIndex;

    public SerializableMap<int> Regions => _regions;

    public MapRegions(SerializableMap<bool> map, int maxRegionSize)
    {
      _map = map;
      _maxRegionSize = maxRegionSize;
      _regions = new SerializableMap<int>(map.width, map.height);

      ScanRegions();
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
  }
}