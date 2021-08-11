using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Map
{
  public class MapGenerator : MonoBehaviour
  {
    [Header("Seeding")]
    [Range(0, 128)]
    public int width;
    
    [Range(0, 128)]
    public int height;

    [Range(0.0f, 1.0f)]
    public double fillChance = 0.5f;

    public int seed = 0;
    public bool randomizeSeed = false;

    [Header("Cellular Automata")]
    public int lowerThreshold = 4;
    public int upperThreshold = 4;
    public int passes = 5;

    [Header("Processing")]
    public int minimumRegionSize = 20;

    private int[,] _map;
    private int[,] _buffer;
    private Random _random;

    private List<MapRegion> _regions;
    private List<MapRegionConnection> _regionConnections;

    private void Start()
    {
      Init();
      
      for (var i = 0; i < passes; i++)
      {
        Simulate();
      }
      
      ScanRegions();
      FillSmallRegions();
      ConnectRegionsWithClosest();
    }

    private void Init()
    {
      if (randomizeSeed)
      {
        seed = DateTime.Now.GetHashCode();
      }
      
      _random = new Random(seed);
      _map = new int[width, height];
      _buffer = new int[width, height];
      _regions = new List<MapRegion>();
      _regionConnections = new List<MapRegionConnection>();

      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          _map[x, y] = x == 0 || y == 0 || x == width - 1 || y == height - 1 || _random.NextDouble() < fillChance ? 1 : 0;
        }
      }
    }

    private int CountWalls(int cellX, int cellY)
    {
      var count = 0;

      for (var y = cellY - 1; y <= cellY + 1; y++)
      {
        for (var x = cellX - 1; x <= cellX + 1; x++)
        {
          if (x < 0 || y < 0 || x >= width || y >= height || _map[x, y] == 1)
          {
            count++;
          }
        }
      }
      
      return count;
    }
    
    private void Simulate()
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          var wallCount = CountWalls(x, y);

          var tile = _map[x, y];
          
          if (wallCount < lowerThreshold)
          {
            tile = 0;
          }
          else if (wallCount > upperThreshold)
          {
            tile = 1;
          }

          _buffer[x, y] = tile;
        }
      }

      (_buffer, _map) = (_map, _buffer);
    }

    private void ScanRegions()
    {
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          _buffer[x, y] = 0;
        }
      }
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          if (_buffer[x, y] == 0 && _map[x, y] == 0)
          {
            _regions.Add(new MapRegion(MapPos.At(x, y), _map, _buffer, 0));
          }
        }
      }

      _regions.Sort();
    }

    private void FillSmallRegions()
    {
      foreach (var region in _regions.ToArray())
      {
        if (region.Size < minimumRegionSize)
        {
          foreach (var cell in region.cells)
          {
            _map[cell.x, cell.y] = 1;
          }

          _regions.Remove(region);
        }
      }
    }

    private void ConnectRegionsWithClosest()
    {
      foreach (var regionA in _regions)
      {
        if (regionA.connectedRegions.Count == 0)
        {
          var bestTileA = MapPos.At(0, 0);
          var bestTileB = MapPos.At(0, 0);
          var distance = int.MaxValue;
          MapRegion bestRegionB = null;

          foreach (var regionB in _regions)
          {
            if (regionA != regionB)
            {
              foreach (var outlineCellA in regionA.outline)
              {
                foreach (var outlineCellB in regionB.outline)
                {
                  var dx = outlineCellB.x - outlineCellA.x;
                  var dy = outlineCellB.y - outlineCellA.y;
                  var d = dx * dx + dy * dy;

                  if (d < distance)
                  {
                    distance = d;
                    bestTileA = outlineCellA;
                    bestTileB = outlineCellB;
                    bestRegionB = regionB;
                  }
                }
              }
            }
          }

          if (bestRegionB != null)
          {
            _regionConnections.Add(new MapRegionConnection(regionA, bestTileA, bestRegionB, bestTileB));
          }
        }
      }
    }

    private static readonly Color[] Colors = new[]
    {
      Color.red,
      Color.blue,
      Color.magenta,
      Color.yellow, 
      Color.cyan
    };
    private void OnDrawGizmos()
    {
      if (_map == null)
      {
        return;
      }
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          Gizmos.color = _map[x, y] == 1 ? Color.black : Color.white;
          Gizmos.DrawCube(new Vector3(x + 0.5f, 0.0f, y + 0.5f), Vector3.one);
        }
      }

      for (var i = 0; i < _regions.Count; i++)
      {
        var region = _regions[i];
        var outline = region.outline;

        Gizmos.color = Colors[i % Colors.Length];
        foreach (var tile in outline)
        {
          Gizmos.DrawCube(new Vector3(tile.x + 0.5f, 1.0f, tile.y + 0.5f), Vector3.one);
        }
      }

      Gizmos.color = Color.green;
      foreach (var connection in _regionConnections)
      {
        var from = connection.PosA;
        var to = connection.PosB;
        Gizmos.DrawLine(new Vector3(from.x + 0.5f, 1.0f, from.y + 0.5f), new Vector3(to.x + 0.5f, 1.0f, to.y + 0.5f));
      }
    }
  }
}