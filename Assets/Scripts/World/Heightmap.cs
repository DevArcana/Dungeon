using System;
using ProceduralNoiseProject;
using UnityEngine;

namespace World
{
  public class Heightmap : MonoBehaviour
  {
    public int seed = 0;
    public float jitter = 1.0f;
    public float frequency = 1.0f;
    public int height = 4;

    public int smoothings = 5;

    private int _sizeX;
    private int _sizeY;
    private int _offsetX;
    private int _offsetY;

    private int[,] _map;
    private int[,] _buffer;

    public int GetHeightAt(int x, int y)
    {
      x -= _offsetX;
      y -= _offsetY;

      if (x < 0 || y < 0 || x >= _sizeX || y >= _sizeY)
      {
        return height;
      }
      
      return _map[x, y];
    }

    private int GetWallCount(int x, int y, int level)
    {
      var count = 0;

      for (var dy = y - 1; dy <= y + 1; dy++)
      {
        for (var dx = x - 1; dx <= x + 1; dx++)
        {
          if ((dx != x || dy != y) && _map[dx, dy] >= level)
          {
            count++;
          }
        }
      }
      
      return count;
    }

    private void Smooth(int level)
    {
      for (var y = 0; y < _sizeY; y++)
      {
        for (var x = 0; x < _sizeX; x++)
        {
          if (x == 0 || x == _sizeX - 1 || y == 0 || y == _sizeY - 1)
          {
            _buffer[x, y] = height;
          }
          else if (_map[x, y] <= level)
          {
            var wallCount = GetWallCount(x, y, level);

            if (wallCount > 4)
            {
              _buffer[x, y] = level;
            }
            else if (wallCount < 2)
            {
              _buffer[x, y] = Math.Max(0, _map[x, y] - 1);
            }
            else
            {
              _buffer[x, y] = _map[x, y];
            }
          }
          else
          {
            _buffer[x, y] = _map[x, y];
          }
        }
      }

      (_buffer, _map) = (_map, _buffer);
    }

    public void Generate(int sizeX, int sizeY, int offsetX, int offsetY)
    {
      _sizeX = sizeX;
      _sizeY = sizeY;
      _offsetX = offsetX;
      _offsetY = offsetY;

      _map = new int[sizeX, sizeY];
      _buffer = new int[sizeX, sizeY];

      var noise = new WorleyNoise(seed, frequency, jitter, height);

      for (var y = 0; y < sizeY; y++)
      {
        for (var x = 0; x < sizeX; x++)
        {
          if (x == 0 || x == sizeX - 1 || y == 0 || y == sizeY - 1)
          {
            _map[x, y] = height;
          }
          else
          {
            _map[x, y] = Mathf.Clamp(Mathf.RoundToInt(noise.Sample2D((float)x / sizeX, (float)y / sizeY)), 0, height);
          }
        }
      }

      for (var i = height; i >= 0; i--)
      {
        for (var j = 0; j < smoothings; j++)
        {
          Smooth(i);
        }
      }
    }
  }
}