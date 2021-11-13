using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Common
{
  [Serializable]
  public struct GridPos : IEquatable<GridPos>
  {
    public bool Equals(GridPos other)
    {
      return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
      return obj is GridPos other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (x * 397) ^ y;
      }
    }

    public static bool operator ==(GridPos left, GridPos right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(GridPos left, GridPos right)
    {
      return !left.Equals(right);
    }

    public int x;
    public int y;

    public GridPos(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public static GridPos At(int x, int y) => new GridPos(x, y);
    public GridPos North => At(x, y + 1);
    public GridPos East => At(x + 1, y );
    public GridPos South => At(x, y - 1);
    public GridPos West => At(x - 1, y);
    
    public int OneDimDistance(GridPos other) => Mathf.Max(Mathf.Abs(this.x - other.x), Mathf.Abs(this.y - other.y));
    public float TwoDimDistance(GridPos other) => Mathf.Sqrt(Mathf.Abs(x - other.x) * Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) * Mathf.Abs(y - other.y));

    public IEnumerable<GridPos> CardinalPattern(int radius, bool wallsBlock = false, bool enemiesBlock = false, bool includeStart = true)
    {
      var world = World.instance;
      
      var tiles = new List<GridPos>();
      if (includeStart)
      {
        tiles.Add(this);
      }

      var currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.East;
        if (world.IsOccupied(currentTile) && enemiesBlock)
        {
          break;
        }

        if (world.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (wallsBlock)
        {
          break;
        }
      }

      currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.West;
        if (world.IsOccupied(currentTile) && enemiesBlock)
        {
          break;
        }

        if (world.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (wallsBlock)
        {
          break;
        }
      }

      currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.North;
        if (world.IsOccupied(currentTile) && enemiesBlock)
        {
          break;
        }

        if (world.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (wallsBlock)
        {
          break;
        }
      }

      currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.South;
        if (world.IsOccupied(currentTile) && enemiesBlock)
        {
          break;
        }

        if (world.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (wallsBlock)
        {
          break;
        }
      }

      return tiles;
    }

    public IEnumerable<GridPos> SquarePattern(int radius)
    {
      var world = World.instance;
      
      var tiles = new List<GridPos>();

      for (var iy = y - radius; iy <= y + radius; iy++)
      {
        for (var ix = x - radius; ix <= x + radius; ix++)
        {
          var currentTile = At(ix, iy);
          if (world.IsWalkable(currentTile))
          {
            tiles.Add(currentTile);
          }
        }
      }

      return tiles;
    }

    public IEnumerable<GridPos> CirclePattern(int radius)
    {
      var boundingBox = SquarePattern(radius);

      var tiles = new List<GridPos>();
      // ReSharper disable once LoopCanBeConvertedToQuery - it cannot lol
      foreach (var tile in boundingBox)
      {
        if (TwoDimDistance(tile) <= radius + 0.5)
        {
          tiles.Add(tile);
        }
      }

      return tiles;
    }
  }
}