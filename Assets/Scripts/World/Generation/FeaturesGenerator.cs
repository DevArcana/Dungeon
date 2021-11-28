using System;
using System.Collections.Generic;
using System.Linq;
using World.Common;

namespace World.Generation
{
  public class FeaturesGenerator
  {
    private readonly byte _maxHeight;
    private readonly SerializableMap<bool> _map;
    private readonly SerializableMap<bool> _mask;
    private readonly HeightMap _heightMap;
    private readonly Random _random;

    // ReSharper disable once ConvertToAutoProperty
    public HeightMap Result => _heightMap;
    
    public FeaturesGenerator(SerializableMap<bool> map, byte maxHeight, Random random)
    {
      _map = map;
      _mask = new SerializableMap<bool>(map.width, map.height);
      _maxHeight = maxHeight;
      _random = random;
      _heightMap = new HeightMap(map.width, map.height);

      for (var y = 0; y < map.height; y++)
      {
        for (var x = 0; x < map.width; x++)
        {
          if (_map[x, y])
          {
            _heightMap[x, y] = _maxHeight;
          }
        }
      }
    }

    public bool Mask(List<GridPos> cells)
    {
      foreach (var cell in cells)
      {
        if (_mask[cell])
        {
          return false;
        }
      }
      
      foreach (var cell in cells)
      {
        _mask[cell] = true;
      }

      return true;
    }

    public void PopulateRegions(RegionsMap regions)
    {
      foreach (var region in regions.AllRegions())
      {
        foreach (var cell in region.cells)
        {
          var width = _random.Next(1, _maxHeight - 2);
          var height = _random.Next(1, _maxHeight - 2);
          var layers = _random.Next(_maxHeight - 2);

          var start = cell;
          var aabb = start.AxisAlignedRect(start.Shift(width + 1, height + 1)).ToList();

          if (regions.Contains(region.index, aabb) && Mask(aabb))
          {
            while (layers > 0)
            {
              layers--;

              if (_random.Next() % 2 == 0)
              {
                if (width > 1)
                {
                  width--;

                  if (width == 0)
                  {
                    break;
                  }
                  if (_random.Next() % 2 == 0)
                  {
                    start = start.West;
                  }
                }
              }
              else
              {
                if (height > 1)
                {
                  height--;
                  
                  if (height == 0)
                  {
                    break;
                  }
                  if (_random.Next() % 2 == 0)
                  {
                    start = start.North;
                  }
                }
              }

              foreach (var tile in start.AxisAlignedRect(start.Shift(width, height)))
              {
                _heightMap[tile]++;
              }
            }
          }
        }
      }
    }
  }
}