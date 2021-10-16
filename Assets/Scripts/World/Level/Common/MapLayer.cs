using System;
using Random = System.Random;

namespace World.Level.Common
{
  public class MapLayer
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

    public MapLayer(int width, int height)
    {
      this.width = width;
      this.height = height;

      _data = new bool[width, height];
    }
    
    public MapLayer(bool[,] data)
    {
      width = data.GetLength(0);
      height = data.GetLength(1);
      _data = data;
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
    public MapLayer Copy()
    {
      var map = new MapLayer(width, height);

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
    /// Returns a number of walls surrounding a given cell.
    /// </summary>
    /// <param name="x">x coordinate</param>
    /// <param name="y">y coordinate</param>
    /// <param name="r">radius</param>
    /// <returns>number of walls</returns>
    public int CountWallsInRadius(int x, int y, int r)
    {
      var count = 0;
      
      for (var j = y - r; j <= y + r; j++)
      {
        for (var i = x - r; i <= x + r; i++)
        {
          if (i == x && j == y) continue;

          if (this[i, j])
          {
            count++;
          }
        }
      }

      return count;
    }

    /// <summary>
    /// Copies the data from the provided map
    /// </summary>
    /// <remarks>Source map must be of the same size!</remarks>
    /// <param name="mapLayer">source map</param>
    /// <exception cref="ArgumentException">thrown if the source map is not of the same size</exception>
    public void CopyFrom(MapLayer mapLayer)
    {
      if (mapLayer.width != width || mapLayer.height != height)
      {
        throw new ArgumentException("provided map must be of the same dimensions!", nameof(mapLayer));
      }
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          _data[x, y] = mapLayer._data[x, y];
        }
      }
    }

    /// <summary>
    /// Randomly fills the layer up to specified fill percentage (approx)
    /// </summary>
    /// <remarks>only affects empty tiles</remarks>
    /// <param name="random">a pseudo random number generator instance</param>
    /// <param name="fillChance">chance in percent each tile will be filled</param>
    public void Randomize(Random random, float fillChance)
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          _data[x, y] = _data[x, y] || random.NextDouble() * 100.0f < fillChance;
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