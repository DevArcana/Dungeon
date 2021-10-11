using System;

namespace World.Map.Generation
{
  public class MapGenerator
  {
    private readonly MapGenerationSettings _settings;
    private readonly Random _random;

    public MapGenerator(MapGenerationSettings settings)
    {
      if (settings == null)
      {
        throw new NullReferenceException("Please provide valid map generation settings!");
      }
      
      _settings = settings;
      _random = _settings.seed != 0 ? new Random(settings.seed) : new Random();
    }

    public Map Generate()
    {
      var map = new Map(_settings.width, _settings.height, _settings.layers);

      // outer walls
      for (var i = 0; i < map.width; i++)
      {
        map[i, 0] = _settings.layers;
        map[i, map.height - 1] = _settings.layers;
      }
      
      for (var i = 0; i < map.height; i++)
      {
        map[0, i] = _settings.layers;
        map[map.width - 1, i] = _settings.layers;
      }
      
      // seed the first level with random noise
      var layer = map.GetLayerAt(0);
      layer.Randomize(_random, _settings.fillPercent);

      var ca = new CellularAutomata(layer);

      for (var i = 0; i < 5; i++)
      {
        ca.Apply(4, 5);
      }

      layer = ca.Result;
      
      map.ApplyLayerAt(layer, 0);

      return map;
    }
  }
}