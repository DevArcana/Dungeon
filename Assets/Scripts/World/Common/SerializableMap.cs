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

    public bool WithinBounds(GridPos pos)
    {
      return WithinBounds(pos.x, pos.y);
    }
    
    public bool WithinBounds(int x, int y)
    {
      return x >= 0 && x < width && y >= 0 && y < height;
    }

    public T this[int x, int y]
    {
      get => data[x + y * width];
      set => data[x + y * width] = value;
    }
  }
}