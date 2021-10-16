using System;
using UnityEngine;

namespace World.Common
{
  public readonly struct GridPos : IEquatable<GridPos>
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

    public readonly int x;
    public readonly int y;

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
  }
}