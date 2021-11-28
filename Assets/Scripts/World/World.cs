using System;
using System.Collections.Generic;
using System.Linq;
using EntityLogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using World.Common;
using World.Generation;
using World.Triggers;

namespace World
{
  public class World : MonoBehaviour
  {
    public static World instance;
    private MapDataProvider _mapDataProvider;

    public int MapWidth => _mapDataProvider.heightmap.width;
    public int MapHeight => _mapDataProvider.heightmap.height;

    /// <summary>
    /// Gets height of the heightmap at specified coordinates.
    /// </summary>
    /// <param name="pos">position on the grid</param>
    /// <returns>height at given point</returns>
    public byte GetHeightAt(GridPos pos) => _mapDataProvider.heightmap.WithinBounds(pos) ? _mapDataProvider.heightmap[pos.x, pos.y] : byte.MaxValue;

    public GridEntity GetEntity(GridPos pos)
    {
      var worldPos = MapUtils.ToWorldPos(pos);
      var colliders = Physics.OverlapBox(worldPos, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("Entities"));
      if (colliders == null || !colliders.Any()) return null;
      return colliders[0].GetComponent<GridEntity>();
    }

    /// <summary>
    /// Determines whether a tile is possible to be walked on.
    /// </summary>
    /// <remarks>Checks whether a tile contains colliders on the "entities" layer.</remarks>
    /// <param name="pos">position to check</param>
    /// <returns>boolean indicating whether a tile is walkable</returns>
    public bool IsOccupied(GridPos pos)
    {
      var worldPos = MapUtils.ToWorldPos(pos);
      return Physics.CheckBox(worldPos, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("Entities"));
    }

    public GridLivingEntity GetOccupant(GridPos pos)
    {
      var worldPos = MapUtils.ToWorldPos(pos);

      var colliders = Physics.OverlapBox(worldPos, Vector3.one * 0.25f, Quaternion.identity, LayerMask.GetMask("Entities"));
      return colliders?.Select(x => x.GetComponent<GridLivingEntity>()).FirstOrDefault(x => x != null);
    }
    
    public IEnumerable<GridTriggerEntity> GetTriggers(GridPos pos)
    {
      var worldPos = MapUtils.ToWorldPos(pos);
      var colliders = Physics.OverlapBox(worldPos, Vector3.one * 0.25f, Quaternion.identity, LayerMask.GetMask("Triggers"));
      return colliders?.Select(x => x.GetComponent<GridTriggerEntity>()).Where(x => x != null);
    }
    
    public bool IsWalkable(GridPos pos)
    {
      var worldPos = MapUtils.ToWorldPos(pos);
      return Math.Abs(worldPos.y - _mapDataProvider.settings.layers) > 0.01;
    }

    private void Start()
    {
      _mapDataProvider = FindObjectOfType<MapDataProvider>();
    }

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else
      {
        Destroy(gameObject);
        return;
      }
      
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      _mapDataProvider = FindObjectOfType<MapDataProvider>();

      if (_mapDataProvider != null)
      {
        _mapDataProvider.Generate();
      }
    }

    public int GetRegionIndex(GridPos pos)
    {
      return _mapDataProvider.regions.WithinBounds(pos) ? _mapDataProvider.regions[pos] : -1;
    }
    
    public Region GetRegion(int index)
    {
      return _mapDataProvider.regions.GetRegion(index);
    }

    public Dictionary<int, Region> AllRegions()
    {
      var regions = new Dictionary<int, Region>();

      foreach (var region in _mapDataProvider.regions.AllRegions())
      {
        regions[region.index] = region;
      }
      
      return regions;
    }
  }
}