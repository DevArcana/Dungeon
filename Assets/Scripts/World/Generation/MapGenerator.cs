using System;
using World.Common;

namespace World.Generation
{
  public class MapGenerationResult
  {
    public HeightMap heightmap;
  }
  
  public class MapGenerator
  {
    private readonly MapGenerationSettings _settings;
    private readonly Random _random;

    public MapGenerator(MapGenerationSettings settings)
    {
      _settings = settings;
      _random = new Random(settings.seed != 0 ? settings.seed : Guid.NewGuid().GetHashCode());
    }

    public MapGenerationResult Generate()
    {
      var heightmap = new HeightMap(_settings.width, _settings.height);

      // step 1, randomize top layer
      var map = heightmap.SliceAt(1);
      for (var y = 0; y < _settings.height; y++)
      {
        for (var x = 0; x < _settings.width; x++)
        {
          if (_random.NextDouble() < _settings.fillPercent * 0.01f)
          {
            map[x, y] = true;
          }
        }
      }

      var ca = new CellularAutomata(map);
      
      ca.Apply();
      // for (var i = 0; i < 5; i++)
      // {
      //   ca.Apply();
      // }

      map = ca.Result;
      
      heightmap.ApplyAt(map, _settings.layers);

      return new MapGenerationResult
      {
        heightmap = heightmap
      };
    }
  }
}