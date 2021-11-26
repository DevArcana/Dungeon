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

    public override string ToString()
    {
      return $"({x}, {y})";
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

    public GridPos Shift(int dx, int dy) => At(x + dx, y + dy);
    
    public int OneDimDistance(GridPos other) => Mathf.Max(Mathf.Abs(x - other.x), Mathf.Abs(y - other.y));
    public float TwoDimDistance(GridPos other) => Mathf.Sqrt(Mathf.Abs(x - other.x) * Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) * Mathf.Abs(y - other.y));

    public IEnumerable<GridPos> AxisAlignedRect(GridPos other)
    {
      var dx = Math.Abs(this.x - other.x);
      var dy = Math.Abs(this.y - other.y);

      for (var x = 0; x < dx; x++)
      {
        for (var y = 0; y < dy; y++)
        {
          yield return At(this.x + x, this.y + y);
        }
      }
    }
  }
}