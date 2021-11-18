using System;
using World.Common;

namespace World.Generation
{
  public class MapGenerationResult
  {
    public HeightMap heightmap;
    public RegionsMap regions;
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
      // step 1, randomize top layer
      var map = new SerializableMap<bool>(_settings.width, _settings.height);
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

      // step 2, apply ca
      var ca = new CellularAutomata(map);
      
      for (var i = 0; i < 5; i++)
      {
        ca.Apply();
      }

      map = ca.Result;
      
      // step 3, ensure connectedness
      var pruning = new MapRegionPruning(map);
      pruning.Scan();
      pruning.PruneRegions(_settings.minRegionSize);
      pruning.ConnectRegions();

      // step 4, split into regions
      var regions = new MapRegions(map, _settings.maxRegionSize, _settings.minRegionSize);

      // step 5, terrain features
      var features = new FeaturesGenerator(map, _settings.layers, _random);
      features.PopulateRegions(regions.Regions);

      return new MapGenerationResult
      {
        heightmap = features.Result,
        regions = regions.Regions
      };
    }
  }
}