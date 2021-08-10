using System;
using UnityEngine;
using Random = System.Random;

namespace World
{
  public class WorldDataProvider : MonoBehaviour
  {
    public int[] fillPercentPerLevel;
    public int seed;
    
    private int _width, _height, _amplitude;
    private Voxel[,,] _data;
    private VoxelType[,] _buffer;

    private Random _random;

    private static readonly Voxel Air = new Voxel() { type = VoxelType.Air };
    private static readonly Voxel Rock = new Voxel() { type = VoxelType.Rock };

    public Voxel GetVoxel(int x, int y, int z)
    {
      if (y >= _amplitude)
      {
        return Air;
      }
      
      if (x < 0 || x >= _width || z < 0 || z >= _height || y < 0)
      {
        return Rock;
      }

      return _data == null ? Air : _data[x, y, z];
    }

    private int CountWalls(int x, int z)
    {
      var count = 0;

      for (var i = x - 1; i <= x + 1; i++)
      {
        for (var j = z - 1; j <= z + 1; j++)
        {
          if (i < 0 || j < 0 || i >= _width || j >= _height || _buffer[i, j] != VoxelType.Air)
          {
            count++;
          }
        }
      }
      
      return count;
    }
    
    private void Generate(int level, int fillPercent)
    {
      for (var x = 0; x < _width; x++)
      {
        for (var z = 0; z < _height; z++)
        {
          if (level < _amplitude - 1 && _data[x, level + 1, z].type != VoxelType.Air)
          {
            _data[x, level, z].type = _data[x, level + 1, z].type;
          }
          else
          {
            var type = _data[x, level, z].type;

            if (type == VoxelType.Air && _random.Next(0, 100) < fillPercent)
            {
              type = VoxelType.Rock;
            }

            _data[x, level, z].type = type;
          }
        }
      }
    }
    
    private void SmoothPass(int level)
    {
      for (var x = 0; x < _width; x++)
      {
        for (var z = 0; z < _height; z++)
        {
          _buffer[x, z] = _data[x, level, z].type;
          
          if (level < _amplitude - 1 && _data[x, level + 1, z].type != VoxelType.Air)
          {
            _buffer[x, z] = _data[x, level + 1, z].type;
          }
        }
      }
      
      for (var x = 0; x < _width; x++)
      {
        for (var z = 0; z < _height; z++)
        {
          if (level < _amplitude - 1 && _data[x, level + 1, z].type != VoxelType.Air)
          {
            continue;
          }
          
          var type = _data[x, level, z].type;
          var wallCount = CountWalls(x, z);

          if (wallCount < 4)
          {
            type = VoxelType.Air;
          }
          else if (wallCount > 4)
          {
            type = VoxelType.Rock;
          }
          
          _data[x, level, z].type = type;
        }
      }
    }
    
    public void Generate(int width, int height, int amplitude)
    {
      if (_random == null)
      {
        _random = new Random(seed);
      }
      
      _width = width;
      _height = height;
      _amplitude = amplitude;

      _data = new Voxel[width, amplitude, height];
      _buffer = new VoxelType[width, height];
      
      for (var y = 0; y < _amplitude; y++)
      {
        for (var x = 0; x < _width; x++)
        {
          for (var z = 0; z < _height; z++)
          {
            _data[x, y, z] = new Voxel() { type = VoxelType.Air };
          }
        }
      }

      for (var y = _amplitude - 1; y >= 0; y--)
      {
        Generate(y, fillPercentPerLevel[y]);

        for (var i = 0; i < 5; i++)
        {
          SmoothPass(y);
        }
      }
    }
  }
}