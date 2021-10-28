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

    public MapGenerator(MapGenerationSettings settings)
    {
      _settings = settings;
    }

    public MapGenerationResult Generate()
    {
      var heightmap = new HeightMap(_settings.width, _settings.height);
      
      return new MapGenerationResult
      {
        heightmap = heightmap
      };
    }
  }
}