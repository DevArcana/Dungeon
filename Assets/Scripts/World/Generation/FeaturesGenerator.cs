using System;
using World.Common;

namespace World.Generation
{
  public class FeaturesGenerator
  {
    private readonly byte _maxHeight;
    private readonly SerializableMap<bool> _map;
    private readonly HeightMap _heightMap;
    private readonly Random _random;

    // ReSharper disable once ConvertToAutoProperty
    public HeightMap Result => _heightMap;
    
    public FeaturesGenerator(SerializableMap<bool> map, byte maxHeight, Random random)
    {
      _map = map;
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

    public void PopulateRegions(RegionsMap regions)
    {
      foreach (var region in regions.AllRegions())
      {
        foreach (var cell in region.cells)
        {
          var height = _random.Next(_maxHeight - 1);
          
          if (regions.Contains(region.index, cell.CirclePattern(height)))
          {
            while (height > 0)
            {
              height--;
              
              foreach (var pos in cell.CirclePattern(height))
              {
                _heightMap[pos]++;
              }
            }
          }
        }
      }
    }
  }
}