using System;

namespace EntityLogic.AI
{
    public class PathNode : IEquatable<PathNode>
    {
        public readonly int x;
        public readonly int y;
        public float gCost;
        public float hCost;
        public float fCost;

        public readonly bool isWalkable;
        public readonly byte height;

        public PathNode previousNode;
        
        public PathNode(int x, int y, byte height, bool isWalkable = true)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.isWalkable = isWalkable;
        }

        public override string ToString()
        {
            return $"({x}, {y}), height {height}, {(isWalkable ? "walkable" : "not walkable")}";
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
        
        public bool Equals(PathNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PathNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        public static bool operator ==(PathNode left, PathNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PathNode left, PathNode right)
        {
            return !Equals(left, right);
        }
    }
}