using System;
using UnityEngine;

namespace World.Map.Generation
{
  [CreateAssetMenu(fileName = "MapSettings", menuName = "Endless Dungeon/Map Generation Settings", order = 1)]
  public class MapGenerationSettings : ScriptableObject
  {
    public int seed = Guid.NewGuid().GetHashCode();
    public int width = 128;
    public int height = 128;
    
    [Min(1)]
    public byte layers = 4;

    public float fillPercent = 45.0f;
  }
}