using System;

namespace Map.Generation
{
  public class Map
  {
    /// <summary>
    /// width of the map
    /// </summary>
    public readonly int width;
    
    /// <summary>
    /// height of the map
    /// </summary>
    public readonly int height;

    private readonly bool[,] _data;

    public Map(int width, int height)
    {
      this.width = width;
      this.height = height;

      _data = new bool[width, height];
    }

    /// <summary>
    /// Checks whether a given coordinate is within the map.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <returns>boolean indicating whether the point (x, y) is in the map</returns>
    public bool IsWithinBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    /// <summary>
    /// Creates a copy of map.
    /// </summary>
    /// <returns>a copy of the map</returns>
    public Map Copy()
    {
      var map = new Map(width, height);

      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          map._data[x, y] = _data[x, y];
        }
      }
      
      return map;
    }

    /// <summary>
    /// Copies the data from the provided map
    /// </summary>
    /// <remarks>Source map must be of the same size!</remarks>
    /// <param name="map">source map</param>
    /// <exception cref="ArgumentException">thrown if the source map is not of the same size</exception>
    public void CopyFrom(Map map)
    {
      if (map.width != width || map.height != height)
      {
        throw new ArgumentException("provided map must be of the same dimensions!", nameof(map));
      }
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          _data[x, y] = map._data[x, y];
        }
      }
    }
    
    public bool this[int x, int y]
    {
      get => IsWithinBounds(x, y) && _data[x, y];
      set => _data[x, y] = value;
    }
  }
}