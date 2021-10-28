using System;

namespace World.Common
{
  [Serializable]
  public class HeightMap : SerializableMap<byte>
  {
    public HeightMap(int width, int height) : base(width, height)
    {
    }

    public SerializableMap<bool> SliceAt(byte level)
    {
      var slice = new SerializableMap<bool>(width, height);

      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          slice[x, y] = this[x, y] >= level;
        }
      }
      
      return slice;
    }

    public void ApplyAt(SerializableMap<bool> map, byte level)
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          if (map[x, y])
          {
            this[x, y] = level;
          }
        }
      }
    }
  }
}