namespace AI
{
    public class PathNode
    {
        public readonly int x;
        public readonly int y;

        public readonly int relativeX;
        public readonly int relativeY;
        
        public float gCost;
        public float hCost;
        public float fCost;

        public readonly bool isWalkable;
        public readonly byte height;

        public PathNode previousNode;
        
        public PathNode(int x, int y, int relativeX, int relativeY, byte height, bool isWalkable = true)
        {
            this.x = x;
            this.y = y;
            this.relativeX = relativeX;
            this.relativeY = relativeY;
            this.height = height;
            this.isWalkable = isWalkable;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}