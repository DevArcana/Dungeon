using System;
using UnityEngine;

namespace World.Level.Common
{
  [Serializable]
  public class Heightmap
  {
    public int width;
    public int height;

    [SerializeField]
    private byte[] heightmap;

    private byte _maxHeight;

    public Heightmap(int width, int height, byte maxHeight = 0)
    {
      this.width = width;
      this.height = height;
      heightmap = new byte[width * height];
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
          data[x, y] = heightmap[x + y * width] == level + 1;
        }
      }

      return new MapLayer(data);
    }

    /// <summary>
    /// Applies a layer at a given level.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="level"></param>
    /// <param name="carve"></param>
    public void ApplyLayerAt(MapLayer layer, byte level, bool carve = false)
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          if (layer[x, y] && !(x == 0 || y == 0 || x == width - 1 || y == height - 1) && heightmap[x + y * width] < level)
          {
            heightmap[x + y * width] = level;
          }
        }
      }
    }
    
    public void CarveLayerTo(MapLayer layer, byte level)
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          if (!layer[x, y] && !(x == 0 || y == 0 || x == width - 1 || y == height - 1) && heightmap[x + y * width] > level)
          {
            heightmap[x + y * width] = level;
          }
        }
      }
    }
    
    public byte this[int x, int y]
    {
      get => IsWithinBounds(x, y) ? heightmap[x + y * width] : _maxHeight;
      set
      {
        heightmap[x + y * width] = value;

        if (value > _maxHeight)
        {
          _maxHeight = value;
        }
      }
    }
  }
}