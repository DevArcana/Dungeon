using System.Collections.Generic;
using Map.Generation;
using UnityEngine;

namespace Map
{
  public class WorldDataProvider : MonoBehaviour
  {
    public static WorldDataProvider Instance { get; private set; }

    public MapGenerator generator;

    public float? GetHeightAt(MapPos pos)
    {
      if (pos.x < 0 || pos.x >= generator.width - 1 || pos.y < 0 || pos.y >= generator.height - 1)
      {
        return null;
      }

      var map = generator.GetMapData();
      var lb = map[pos.x, pos.y];
      var rb = map[pos.x + 1, pos.y];
      var lt = map[pos.x, pos.y + 1];
      var rt = map[pos.x + 1, pos.y + 1];

      var heights = new Dictionary<int, int>();
      heights[lb] = heights.ContainsKey(lb) ? heights[lb] + 1 : 1;
      heights[rb] = heights.ContainsKey(rb) ? heights[rb] + 1 : 1;
      heights[lt] = heights.ContainsKey(lt) ? heights[lt] + 1 : 1;
      heights[rt] = heights.ContainsKey(rt) ? heights[rt] + 1 : 1;

      if (heights[lb] >= 3)
      {
        return lb;
      }
      
      if (heights[rb] >= 3)
      {
        return rb;
      }
      
      if (heights[lt] >= 3)
      {
        return lt;
      }
      
      if (heights[rt] >= 3)
      {
        return rt;
      }
      
      return null;
    }

    private void Awake()
    {
      if (Instance != null)
      {
        Debug.LogWarning("Multiple world data providers in the scene!");
      }

      Instance = this;
    }
  }
}
