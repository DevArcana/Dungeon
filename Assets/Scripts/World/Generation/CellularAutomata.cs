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
          if (tx == x && ty == y) continue;

          if (!_map.WithinBounds(tx, ty))
          {
            count++;
          }
          else if (_map[tx, ty])
          {
            count++;
          }
        }
      }

      return count;
    }

    public void Apply(MapGenerationSettings settings)
    {
      for (var y = 0; y < _map.height; y++)
      {
        for (var x = 0; x < _map.width; x++)
        {
          var r1 = NeighboursInRadius(x, y, 1);
          var r3 = NeighboursInRadius(x, y, settings.rn);

          _buffer[x, y] = _map[x, y];
          
          if (r1 >= settings.r1CellsToLive || r3 <= settings.rnCellsToLive)
          {
            _buffer[x, y] = true;
          }
          else if (r1 <= settings.r1CellsToDie)
          {
            _buffer[x, y] = false;
          }
        }
      }

      (_map, _buffer) = (_buffer, _map);
    }
  }
}