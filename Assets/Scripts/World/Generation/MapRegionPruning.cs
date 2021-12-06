using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World.Common;

namespace World.Generation
{
  public class Connection
  {
    public MapRegion regionA;
    public MapRegion regionB;
    public GridPos a;
    public GridPos b;
  }

  public class MapRegion
  {
    public List<GridPos> cells;
    public List<GridPos> outline;

    public bool connectedToMain;

    public List<Connection> connections = new List<Connection>();

    public void Connect(MapRegion region, GridPos a, GridPos b)
    {
      var connection = new Connection
      {
        a = a,
        b = b,
        regionA = this,
        regionB = region
      };

      connections.Add(connection);
      region.connections.Add(connection);
    }
  }

  public class MapRegionPruning
  {
    private readonly SerializableMap<bool> _map;
    private readonly SerializableMap<int> _visited;
    private readonly Dictionary<int, MapRegion> _regions;

    private int _regionIndex = 0;

    public MapRegionPruning(SerializableMap<bool> map)
    {
      _map = map;
      _visited = new SerializableMap<int>(map.width, map.height);
      _regions = new Dictionary<int, MapRegion>();
    }

    private void ScanRegion(int x, int y)
    {
      _regionIndex++;
      var cells = new List<GridPos>();
      var outline = new List<GridPos>();
      var open = new Queue<GridPos>();
      open.Enqueue(GridPos.At(x, y));

      while (open.Count > 0)
      {
        var pos = open.Dequeue();

        if (!_visited.WithinBounds(pos) || _visited[pos] != 0) continue;
        if (_map[pos]) continue;

        _visited[pos] = _regionIndex;

        cells.Add(pos);

        if (!_map.WithinBounds(pos.North) || _map[pos.North]
                                          || !_map.WithinBounds(pos.East) || _map[pos.East]
                                          || !_map.WithinBounds(pos.South) || _map[pos.South]
                                          || !_map.WithinBounds(pos.West) || _map[pos.West])
        {
          outline.Add(pos);
        }

        open.Enqueue(pos.North);
        open.Enqueue(pos.East);
        open.Enqueue(pos.South);
        open.Enqueue(pos.West);
      }

      _regions[_regionIndex] = new MapRegion
      {
        cells = cells,
        outline = outline
      };
    }

    public void Scan()
    {
      for (var y = 0; y < _map.height; y++)
      {
        for (var x = 0; x < _map.width; x++)
        {
          if (!_map[x, y] && _visited[x, y] == 0)
          {
            ScanRegion(x, y);
          }
        }
      }
    }

    public void PruneRegions(int minimumSize)
    {
      foreach (var regionIndex in _regions.Keys.ToArray())
      {
        var region = _regions[regionIndex];
        if (region.cells.Count < minimumSize)
        {
          foreach (var cell in region.cells)
          {
            _map[cell] = true;
          }

          _regions.Remove(regionIndex);
        }
      }
    }

    private void Carve(int x, int y)
    {
      var pos = GridPos.At(x, y);
      _map[pos] = false;
      if (_map.WithinBounds(pos.North)) _map[pos.North] = false;
      if (_map.WithinBounds(pos.East)) _map[pos.East] = false;
      if (_map.WithinBounds(pos.South)) _map[pos.South] = false;
      if (_map.WithinBounds(pos.West)) _map[pos.West] = false;
    }

    private void Carve(int x0, int y0, int x1, int y1)
    {
      int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
      int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
      int err = dx + dy, e2; /* error value e_xy */
      while (true)
      {
        /* loop */
        Carve(x0, y0);
        e2 = 2 * err;
        if (e2 >= dy)
        {
          /* e_xy+e_x > 0 */
          if (x0 == x1) break;
          err += dy;
          x0 += sx;
        }

        if (e2 <= dx)
        {
          /* e_xy+e_y < 0 */
          if (y0 == y1) break;
          err += dx;
          y0 += sy;
        }
      }
    }

    private void Carve(GridPos a, GridPos b)
    {
      Carve(a.x, a.y, b.x, b.y);
    }

    private void Carve(Connection connection)
    {
      Carve(connection.a, connection.b);
    }

    public void ConnectRegions()
    {
      MapRegion main = null;
      foreach (var region in _regions.Values)
      {
        if (main == null || main.cells.Count < region.cells.Count)
        {
          main = region;
        }
        
        if (region.connections.Count == 0)
        {
          var bestRegionA = region;
          var bestRegionB = region;
          var bestA = region.outline[0];
          var bestB = region.outline[0];
          var bestDistance = float.MaxValue;

          foreach (var other in _regions.Values)
          {
            if (other == region) continue;

            foreach (var a in region.outline)
            {
              foreach (var b in other.outline)
              {
                var distance = a.TwoDimDistance(b);

                if (distance < bestDistance)
                {
                  bestRegionB = other;
                  bestA = a;
                  bestB = b;
                  bestDistance = distance;
                }
              }
            }
          }

          bestRegionA.Connect(bestRegionB, bestA, bestB);
        }
      }

      if (main == null)
      {
        return;
      }
      main.connectedToMain = true;

      var connected = new List<MapRegion>();
      var disconnected = new List<MapRegion>();

      foreach (var region in _regions.Values)
      {
        if (!region.connectedToMain)
        {
          disconnected.Add(region);
        }
        else
        {
          connected.Add(region);
        }
      }

      while (disconnected.Count > 0)
      {
        var bestRegionA = disconnected[0];
        var bestRegionB = disconnected[0];
        var bestA = disconnected[0].outline[0];
        var bestB = disconnected[0].outline[0];
        var bestDistance = float.MaxValue;
        
        foreach (var region in disconnected)
        {
          foreach (var other in connected)
          {
            if (other == region) continue;

            foreach (var a in region.outline)
            {
              foreach (var b in other.outline)
              {
                var distance = a.TwoDimDistance(b);

                if (distance < bestDistance)
                {
                  bestRegionA = region;
                  bestRegionB = other;
                  bestA = a;
                  bestB = b;
                  bestDistance = distance;
                }
              }
            }
          }
        }

        disconnected.Remove(bestRegionA);
        connected.Add(bestRegionA);
        bestRegionA.Connect(bestRegionB, bestA, bestB);
      }
      
      var connections = _regions.Values.SelectMany(r => r.connections).Distinct();
      foreach (var connection in connections)
      {
        if (connection.regionA == connection.regionB) continue;
        Carve(connection);
      }
    }
  }
}