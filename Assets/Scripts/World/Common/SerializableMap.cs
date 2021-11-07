using System;

namespace World.Common
{
  [Serializable]
  public class SerializableMap<T>
  {
    public int width;
    public int height;

    public T[] data;

    public SerializableMap(int width, int height)
    {
      this.width = width;
      this.height = height;
      data = new T[width * height];
    }

    public SerializableMap<T> Copy()
    {
      var copy = new SerializableMap<T>(width, height);
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          copy[x, y] = this[x, y];
        }
      }

      return copy;
    }

    public void Clear()
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          this[x, y] = default;
        }
      }
    }

    public bool WithinBounds(GridPos pos)
    {
      return WithinBounds(pos.x, pos.y);
    }
    
    public bool WithinBounds(int x, int y)
    {
      return x >= 0 && x < width && y >= 0 && y < height;
    }

    public T GetOrDefault(GridPos pos, T fallback = default)
    {
      return GetOrDefault(pos.x, pos.y, fallback);
    }

    private T GetOrDefault(int x, int y, T fallback = default)
    {
      return WithinBounds(x, y) ? this[x, y] : fallback;
    }

    public T this[int x, int y]
    {
      get => data[x + y * width];
      set => data[x + y * width] = value;
    }
    
    public T this[GridPos pos]
    {
      get => data[pos.x + pos.y * width];
      set => data[pos.x + pos.y * width] = value;
    }
  }
}