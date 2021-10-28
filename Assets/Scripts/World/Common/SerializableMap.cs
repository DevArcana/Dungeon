﻿using System;

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