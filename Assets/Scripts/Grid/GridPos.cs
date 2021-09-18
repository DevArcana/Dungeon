namespace Grid
{
  public struct GridPos
  {
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
  }
}