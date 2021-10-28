using UnityEngine;
using World.Common;

namespace World.Generation
{
  public class CellularAutomata
  {
    private SerializableMap<bool> _map;
    private SerializableMap<bool> _buffer;

    public SerializableMap<bool> Result => _map;

    public CellularAutomata(SerializableMap<bool> map)
    {
      _map = map.Copy();
      _buffer = map.Copy();
    }

    private int NeighboursInRadius(int x, int y, int r)
    {
      var count = 0;
      
      for (var ty = y - r; ty <= y + r; ty++)
      {
        for (var tx = x - r; tx <= x + r; tx++)
        {
          if (!_map.WithinBounds(tx, ty) || tx != x && ty != y && _map[tx, ty])
          {
            count += 1;
          }
        }
      }

      return count;
    }

    public void Apply()
    {
      for (var y = 0; y < _map.height; y++)
      {
        for (var x = 0; x < _map.width; x++)
        {
          var r1 = NeighboursInRadius(x, y, 1);
          var r2 = NeighboursInRadius(x, y, 2);

          _buffer[x, y] = _map[x, y];
          
          // if (r1 >= 4)
          // {
          //   _buffer[x, y] = true;
          // }
        }
      }

      (_map, _buffer) = (_buffer, _map);
    }
  }
}