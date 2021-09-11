using System.Collections.Generic;
using Map.Generation;
using UnityEngine;

namespace Map
{
  public class WorldDataProvider : MonoBehaviour
  {
    public static WorldDataProvider Instance { get; private set; }

    public MapGenerator generator;

    public float GetHeightAt(MapPos pos)
    {
      if (pos.x < 0 || pos.x >= generator.width || pos.y < 0 || pos.y >= generator.height)
      {
        return 1;
      }

      return generator.GetMapData()[pos.x, pos.y];
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
