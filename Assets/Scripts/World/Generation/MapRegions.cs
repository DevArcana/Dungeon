using System;
using System.Collections.Generic;
using System.Linq;
using World.Common;

namespace World.Generation
{
  public class MapRegions
  {
    private readonly SerializableMap<bool> _map;
    private readonly RegionsMap _regions;
    private readonly List<GridPos> _unallocated;

    private readonly int _maxRegionSize;
    private readonly int _minRegionSize;

    private readonly Random _random;

    private int _regionIndex;

    public RegionsMap Regions => _regions;

    public MapRegions(SerializableMap<bool> map, int maxRegionSize, int minRegionSize)
    {
      _map = map;
      _maxRegionSize = maxRegionSize;
      _minRegionSize = minRegionSize;
      _regions = new RegionsMap(map.width, map.height);
      _unallocated = new List<GridPos>();
      _random = new Random();
      
      ScanRegions();
    }

    private void ScanRegion(int sx, int sy)
    {
      var queue = new Queue<GridPos>();
      queue.Enqueue(GridPos.At(sx, sy));

      _regionIndex++;

      var size = 0;
      var cells = new List<GridPos>();

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
          cells.Add(pos);

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

      if (size < _minRegionSize)
      {
        foreach (var cell in cells)
        {
          _regions[cell] = -2;
          _unallocated.Add(cell);
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
      
      FillUnallocatedRegions();
    }

    private void FillUnallocatedRegions()
    {
      if (_unallocated.Count == 0)
      {
        return;
      }
      
      var choices = new List<int>(4);
      var allocated = new List<GridPos>(_unallocated.Count);
      
      while (_unallocated.Count > 0)
      {
        foreach (var pos in _unallocated)
        {
          choices.Clear();
          var choice = _regions[pos.North];
          if (choice > 0)
          {
            choices.Add(choice);
          }
          
          choice = _regions[pos.East];
          if (choice > 0)
          {
            choices.Add(choice);
          }
          
          choice = _regions[pos.South];
          if (choice > 0)
          {
            choices.Add(choice);
          }
          
          choice = _regions[pos.West];
          if (choice > 0)
          {
            choices.Add(choice);
          }

          if (choices.Count > 0)
          {
            _regions[pos] = choices[_random.Next(choices.Count)];
            allocated.Add(pos);
          }
        }

        foreach (var pos in allocated)
        {
          _unallocated.Remove(pos);
        }
        allocated.Clear();
      }
    }
  }
}