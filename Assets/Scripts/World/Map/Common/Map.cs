using World.Map.Generation;

namespace World.Map
{
  public class Map
  {
    public readonly int width;
    public readonly int height;

    private readonly byte[,] _heightmap;

    private byte _maxHeight;

    public Map(int width, int height, byte maxHeight = 0)
    {
      this.width = width;
      this.height = height;
      _heightmap = new byte[width, height];
      _maxHeight = maxHeight;
    }

    /// <summary>
    /// Checks whether a given coordinate is within the map.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <returns>boolean indicating whether the point (x, y) is in the map</returns>
    public bool IsWithinBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    /// <summary>
    /// Creates a MapLayer object representing a slice of the map at a given level
    /// </summary>
    /// <param name="level">height at which to slice the map</param>
    /// <returns>a single layer of the map</returns>
    public MapLayer GetLayerAt(byte level)
    {
      var data = new bool[width, height];

      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          data[x, y] = _heightmap[x, y] == level + 1;
        }
      }

      return new MapLayer(data);
    }

    /// <summary>
    /// Applies a layer at a given level.
    /// When the height at said level is equal to the level, it sets the height to either level or one higher depending on layer.
    /// Otherwise, it does nothing.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="level"></param>
    public void ApplyLayerAt(MapLayer layer, byte level)
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          var h = _heightmap[x, y];
          
          if (h == level || h == level + 1)
          {
            h = layer[x, y] ? (byte) (h + 1) : h;
          }

          _heightmap[x, y] = h;
        }
      }
    }
    
    public byte this[int x, int y]
    {
      get => IsWithinBounds(x, y) ? _heightmap[x, y] : _maxHeight;
      set
      {
        _heightmap[x, y] = value;

        if (value > _maxHeight)
        {
          _maxHeight = value;
        }
      }
    }
  }
}