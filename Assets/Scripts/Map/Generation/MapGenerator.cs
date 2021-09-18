using System;
using System.Collections.Generic;
using Grid;
using UnityEngine;
using Random = System.Random;

namespace Map.Generation
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

    public MapGenerator previous;

    [Header("Cellular Automata")]
    public int lowerThreshold = 4;
    public int upperThreshold = 4;
    public int passes = 5;

    [Header("Processing")]
    public int minimumRegionSize = 20;

    [Header("Gizmos")]
    public bool drawMetadataGizmos = true;

    private int[,] _map;
    private int[,] _buffer;
    private Random _random;

    private List<MapRegion> _regions;
    private List<MapRegionConnection> _regionConnections;

    private void Start()
    {
      Generate();
    }
    
    public List<MapRegion> GetRegionData()
    {
      if (_map == null)
      {
        Generate();
      }

      return _regions;
    }

    public int[,] GetMapData()
    {
      if (_map == null)
      {
        Generate();
      }

      return _map;
    }

    private void Generate()
    {
      if (_map != null)
      {
        return;
      }
      
      Init();

      for (var i = 0; i < passes; i++)
      {
        Simulate();
      }

      ScanRegions();
      FillSmallRegions();

      if (_regions.Count > 0)
      {
        _regions.Sort();
        _regions[0].ConnectToRoot();
      }

      ConnectRegionsWithClosest();
      ConnectRegionsToRoot();
      
      CarvePassages();
    }

    private void Init()
    {
      if (randomizeSeed)
      {
        seed = DateTime.Now.GetHashCode();
      }
      
      _random = new Random(seed);
      _buffer = new int[width, height];
      _regions = new List<MapRegion>();
      _regionConnections = new List<MapRegionConnection>();

      if (previous != null)
      {
        _map = previous.GetMapData();
        
        for (var y = 0; y < height; y++)
        {
          for (var x = 0; x < width; x++)
          {
            if (_map[x, y] > 0)
            {
              _map[x, y]++;
            }
          }
        }
      }
      else
      {
        _map = new int[width, height];
      }
      
      for (var y = 0; y < height; y++)
      {
        for (var x = 0; x < width; x++)
        {
          if (_map[x, y] == 1 || _map[x, y] == 0)
          {
            _map[x, y] = x == 0 || y == 0 || x == width - 1 || y == height - 1 || _random.NextDouble() < fillChance ? 1 : 0;
          }
        }
      }
    }

    private bool IsOutsideGrid(int x, int y)
    {
      return x < 0 || y < 0 || x >= width || y >= height;
    }
    
    private bool IsWithinGrid(int x, int y)
    {
      return !IsOutsideGrid(x, y);
    }

    private int CountWalls(int cellX, int cellY)
    {
      var count = 0;

      for (var y = cellY - 1; y <= cellY + 1; y++)
      {
        for (var x = cellX - 1; x <= cellX + 1; x++)
        {
          if (IsOutsideGrid(x, y) || _map[x, y] >= 1)
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

          if (tile == 1 || tile == 0)
          {
            if (wallCount < lowerThreshold)
            {
              tile = 0;
            }
            else if (wallCount > upperThreshold)
            {
              tile = 1;
            }
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
            _regions.Add(new MapRegion(GridPos.At(x, y), _map, _buffer, 0));
          }
        }
      }
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
          var bestTileA = GridPos.At(0, 0);
          var bestTileB = GridPos.At(0, 0);
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

    private void ConnectRegionsToRoot()
    {
      while (true)
      {
        var connectedRegions = new List<MapRegion>();
        var disconnectedRegions = new List<MapRegion>();

        foreach (var region in _regions)
        {
          if (region.IsConnectedToRoot)
          {
            connectedRegions.Add(region);
          }
          else
          {
            disconnectedRegions.Add(region);
          }
        }

        var bestTileA = GridPos.At(0, 0);
        var bestTileB = GridPos.At(0, 0);
        var distance = int.MaxValue;
        MapRegion bestRegionA = null;
        MapRegion bestRegionB = null;

        foreach (var disconnectedRegion in disconnectedRegions)
        {
          foreach (var connectedRegion in connectedRegions)
          {
            foreach (var outlineCellA in disconnectedRegion.outline)
            {
              foreach (var outlineCellB in connectedRegion.outline)
              {
                var dx = outlineCellB.x - outlineCellA.x;
                var dy = outlineCellB.y - outlineCellA.y;
                var d = dx * dx + dy * dy;

                if (d < distance)
                {
                  distance = d;
                  bestTileA = outlineCellA;
                  bestTileB = outlineCellB;
                  bestRegionA = disconnectedRegion;
                  bestRegionB = connectedRegion;
                }
              }
            }
          }
        }

        if (bestRegionB != null)
        {
          _regionConnections.Add(new MapRegionConnection(bestRegionA, bestTileA, bestRegionB, bestTileB));
          continue;
        }

        break;
      }
    }

    private List<GridPos> GetLine(GridPos from, GridPos to)
    {
      var dx = from.x - to.x;
      var dy = from.y - to.y;
      
      var absX = Math.Abs(dx);
      var absY = Math.Abs(dy);
      
      var signX = Math.Sign(dx);
      var signY = Math.Sign(dy);

      var x = to.x;
      var y = to.y;
      
      var cells = new List<GridPos>() { GridPos.At(x, y) };
      
      for (int i = 0, j = 0; i < absX || j < absY;) {
        var decision = (1 + 2*i) * absY - (1 + 2*j) * absX;
        if (decision == 0) {
          x += signX;
          y += signY;
          i++;
          j++;
        } else if (decision < 0) {
          x += signX;
          i++;
        } else {
          y += signY;
          j++;
        }
        cells.Add(GridPos.At(x, y));
      }

      return cells;
    }

    private void Carve(GridPos pos)
    {
      for (var y = pos.y - 1; y <= pos.y + 1; y++)
      {
        for (var x = pos.x - 1; x <= pos.x + 1; x++)
        {
          if (IsWithinGrid(x, y))
          {
            _map[x, y] = 0;
          }
        }
      }
    }
    
    private void CarvePassage(GridPos from, GridPos to)
    {
      var cells = GetLine(from, to);

      foreach (var cell in cells)
      {
        Carve(cell);
      }
    }
    
    private void CarvePassages()
    {
      foreach (var connection in _regionConnections)
      {
        var from = connection.PosA;
        var to = connection.PosB;
        
        CarvePassage(from, to);
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
          Gizmos.DrawCube(new Vector3(x + 0.5f, 0.5f, y + 0.5f), Vector3.one * 0.5f);
        }
      }

      if (drawMetadataGizmos)
      {
        for (var i = 0; i < _regions.Count; i++)
        {
          var region = _regions[i];
          var outline = region.outline;

          Gizmos.color = Colors[i % Colors.Length];
          foreach (var tile in outline)
          {
            Gizmos.DrawCube(new Vector3(tile.x + 0.5f, 1.0f, tile.y + 0.5f), Vector3.one * 0.45f);
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
}