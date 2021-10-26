using System;
using World.Level.Common;

namespace World.Level.Generation
{
  [Serializable]
  public class MapGenerator
  {
    private readonly MapGenerationSettings _settings;
    private readonly Random _random;

    public Heightmap heightmap;
    public RegionBuilder regionBuilder;

    public MapGenerator(MapGenerationSettings settings)
    {
      if (settings == null)
      {
        throw new NullReferenceException("Please provide valid map generation settings!");
      }
      
      _settings = settings;
      _random = _settings.seed != 0 ? new Random(settings.seed) : new Random();
    }

    public void Generate()
    {
      var map = new Heightmap(_settings.width, _settings.height, _settings.layers);

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

      // cellular automata
      var ca = new CellularAutomata(layer);
      
      for (var i = 0; i < 5; i++)
      {
        ca.Apply(4, 5);
      }
      
      layer = ca.Result;

      // detect regions
      var regions = new RegionBuilder(layer);
      regions.FillSmallRegions(30);
      regions.EnsureConnectedness();
      
      heightmap = map;
      regionBuilder = regions;
      
      layer = regions.Result;
      var topLayer = layer;
      map.ApplyLayerAt(topLayer, _settings.layers);

      for (var i = _settings.layers - 2; i >= 1; i--)
      {
        var copy = layer.Copy();
        copy.Randomize(_random, _settings.fillPercent * 0.5f);
        ca = new CellularAutomata(copy);
        
        for (var j = 0; j < 5; j++)
        {
          ca.Apply(4, 5);
        }
        
        ca.Result.Add(layer);
        layer = ca.Result;

        regions = new RegionBuilder(layer);
        regions.FillSmallRegions(15);
        regions.EnsureConnectedness();
        regions.CarvePassages(topLayer);
        
        map.ApplyLayerAt(layer, (byte) i);
        map.CarveLayerTo(topLayer, (byte) i);
      }
    }
  }
}