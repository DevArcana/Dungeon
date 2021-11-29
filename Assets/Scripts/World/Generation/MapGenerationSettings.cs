using System;
using UnityEngine;

namespace World.Generation
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
    
    public int maxRegionSize = 200;
    public int minRegionSize = 100;
    
    public Transform playerPrefab;
    public Transform staircasePrefab;

    public int iterations = 5;
    public int r1CellsToLive = 5;
    public int r1CellsToDie = 2;
    public int rn = 3;
    public int rnCellsToLive = 3;
  }
}