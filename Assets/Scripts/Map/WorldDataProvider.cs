using Grid;
using Map.Generation;
using UnityEngine;
using Utils;

namespace Map
{
  public class WorldDataProvider : MonoBehaviour
  {
    public static WorldDataProvider Instance { get; private set; }

    public Transform playerSpawnPrefab;
    public Transform enemySpawnPrefab;
    
    public MapGenerator generator;

    private WorldData[,] _data;
    private int _width, _height;

    public float GetHeightAt(GridPos pos)
    {
      if (pos.x < 0 || pos.x >= _width || pos.y < 0 || pos.y >= _height)
      {
        return 0;
      }

      return _data[pos.x, pos.y].height;
    }

    public bool IsWithinWorld(GridPos pos)
    {
      return !(pos.x < 0 || pos.x >= _width || pos.y < 0 || pos.y >= _height);
    }

    public WorldData? GetData(GridPos pos)
    {
      if (!IsWithinWorld(pos))
      {
        return null;
      }

      return _data[pos.x, pos.y];
    }

    private void Awake()
    {
      if (Instance != null)
      {
        Debug.LogWarning("Multiple world data providers in the scene!");
      }

      Instance = this;
    }

    private void Start()
    {
      _width = generator.width;
      _height = generator.height;

      var map = generator.GetMapData();

      _data = new WorldData[_width, _height];
      for (var y = 0; y < _height; y++)
      {
        for (var x = 0; x < _width; x++)
        {
          _data[x, y] = new WorldData(x, y, map[x, y]);
        }
      }

      var regions = generator.GetRegionData();

      var spawnRegion = Random.Range(0, regions.Count);

      for (var regionIndex = 0; regionIndex < regions.Count; regionIndex++)
      {
        var region = regions[regionIndex];
        foreach (var cell in region.cells)
        {
          _data[cell.x, cell.y].regionIndex = regionIndex;
        }

        if (regionIndex == spawnRegion)
        {
          var spawnCell = region.cells[Random.Range(0, region.cells.Count)];
          var pos = MapUtils.ToWorldPos(spawnCell);
          Instantiate(playerSpawnPrefab, pos, Quaternion.identity);
        }
      }
    }
  }
}
