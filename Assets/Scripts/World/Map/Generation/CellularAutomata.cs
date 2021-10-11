namespace World.Map.Generation
{
  public class CellularAutomata
  {
    private MapLayer _buffer;
    private MapLayer _layer;

    public CellularAutomata(MapLayer layer)
    {
      _layer = layer;
      _buffer = layer.Copy();
    }

    private int CountWallsInRadius(int x, int y, int r)
    {
      var count = 0;
      
      for (var j = y - r; j <= y + r; j++)
      {
        for (var i = x - r; i <= x + r; i++)
        {
          if (i == x && j == y) continue;

          if (_layer[i, j])
          {
            count++;
          }
        }
      }

      return count;
    }
    
    public void Apply(int a, int b)
    {
      var w = _layer.width;
      var h = _layer.height;
      
      for (var y = 0; y < h; y++)
      {
        for (var x = 0; x < w; x++)
        {
          var c = CountWallsInRadius(x, y, 1);

          if (_layer[x, y] && c >= a)
          {
            _buffer[x, y] = true;
          }
          else if (c >= b)
          {
            _buffer[x, y] = true;
          }
          else
          {
            _buffer[x, y] = false;
          }
        }
      }
      
      (_layer, _buffer) = (_buffer, _layer);
    }

    // performance reasons
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public MapLayer Result => _layer;
  }
}