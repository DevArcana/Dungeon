using System;
using System.Collections.Generic;
using EntityLogic;
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

    public IEnumerable<GridPos> Pattern(Pattern pattern, int radius, bool respectWalls = false, bool includeStart = true)
    {
      return pattern switch
      {
        Common.Pattern.Cardinal => CardinalPattern(radius, respectWalls, includeStart),
        _ => Array.Empty<GridPos>()
      };
    }

    private IEnumerable<GridPos> CardinalPattern(int radius, bool respectWalls = false, bool includeStart = true)
    {
      var tiles = new List<GridPos>();
      if (includeStart)
      {
        tiles.Add(this);
      }

      var currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.East;
        if (World.instance.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (respectWalls)
        {
          break;
        }
      }

      currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.West;
        if (World.instance.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (respectWalls)
        {
          break;
        }
      }

      currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.North;
        if (World.instance.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (respectWalls)
        {
          break;
        }
      }

      currentTile = this;
      for (var i = 0; i < radius; i++)
      {
        currentTile = currentTile.South;
        if (World.instance.IsWalkable(currentTile))
        {
          tiles.Add(currentTile);
        }
        else if (respectWalls)
        {
          break;
        }
      }

      return tiles;
    }
  }

  public enum Pattern
  {
    Cardinal
  }
}