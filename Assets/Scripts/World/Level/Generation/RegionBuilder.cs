using System;
using System.Collections.Generic;
using UnityEngine;
using World.Common;
using World.Level.Common;

namespace World.Level.Generation
{
  [Serializable]
  public class RegionBuilder
  {
    [SerializeField]
    private List<MapRegion> regions = new List<MapRegion>();
    
    [SerializeField]
    private List<MapRegionConnection> regionConnections = new List<MapRegionConnection>();

    private readonly MapLayer _layer;

    public IEnumerable<MapRegion> Regions => regions;
    public IEnumerable<MapRegionConnection> Connections => regionConnections;

    // performance reasons
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public MapLayer Result => _layer;
    
    public RegionBuilder(MapLayer map)
    {
      _layer = map.Copy();
      var mask = new MapLayer(map.width, map.height);

      for (var y = 0; y < map.height; y++)
      {
        for (var x = 0; x < map.width; x++)
        {
          if (!mask[x, y] && !map[x, y])
          {
            regions.Add(new MapRegion(GridPos.At(x, y), map, mask));
          }
        }
      }
      
    }
    
    public void FillSmallRegions(int minimumRegionSize)
    {
      foreach (var region in regions.ToArray())
      {
        if (region.Size < minimumRegionSize)
        {
          foreach (var cell in region.cells)
          {
            _layer[cell.x, cell.y] = true;
          }

          regions.Remove(region);
        }
      }
    }

    public void EnsureConnectedness(MapLayer layer = null)
    {
      layer ??= _layer;
      
      if (regions.Count > 0)
      {
        regions.Sort();
        regions[0].ConnectToRoot();
      }
      
      ConnectRegionsWithClosest();
      ConnectRegionsToRoot();
      CarvePassages(layer);
    }

    private void ConnectRegionsWithClosest()
    {
      foreach (var regionA in regions)
      {
        if (regionA.connectedRegions.Count == 0)
        {
          var bestTileA = GridPos.At(0, 0);
          var bestTileB = GridPos.At(0, 0);
          var distance = int.MaxValue;
          MapRegion bestRegionB = null;

          foreach (var regionB in regions)
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
            regionConnections.Add(new MapRegionConnection(regionA, bestTileA, bestRegionB, bestTileB));
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

        foreach (var region in regions)
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
          regionConnections.Add(new MapRegionConnection(bestRegionA, bestTileA, bestRegionB, bestTileB));
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

    private void Carve(MapLayer layer, GridPos pos)
    {
      for (var y = pos.y - 1; y <= pos.y + 1; y++)
      {
        for (var x = pos.x - 1; x <= pos.x + 1; x++)
        {
          if (layer.IsWithinBounds(x, y))
          {
            layer[x, y] = false;
          }
        }
      }
    }
    
    private void CarvePassage(MapLayer layer, GridPos from, GridPos to)
    {
      var cells = GetLine(from, to);

      foreach (var cell in cells)
      {
        Carve(layer, cell);
      }
    }
    
    public void CarvePassages(MapLayer layer)
    {
      foreach (var connection in regionConnections)
      {
        var from = connection.PosA;
        var to = connection.PosB;
        
        CarvePassage(layer, from, to);
      }
    }
  }
}