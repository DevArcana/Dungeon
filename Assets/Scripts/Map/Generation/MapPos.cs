namespace Map.Generation
{
  public struct MapPos
  {
    public int x;
    public int y;

    public MapPos(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public static MapPos At(int x, int y) => new MapPos(x, y);
  }
}