namespace WorldGeneration
{
    public class VoxelStencil
    {
        private bool fillType;
        private int centerX, centerY, radius;

        public void Initialize(bool fillType, int radius)
        {
            this.fillType = fillType;
            this.radius = radius;
        }

        public void SetCenter(int x, int y)
        {
            centerX = x;
            centerY = y;
        }

        public int XStart => centerX - radius;
        public int XEnd => centerX + radius;
        
        public int YStart => centerY - radius;
        public int YEnd => centerY + radius;

        public bool Apply(int x, int y)
        {
            return fillType;
        }
    }
}