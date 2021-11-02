﻿using System.Collections.Generic;
using System.Linq;
using EntityLogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using World.Common;
using World.Generation;

namespace World
{
  public class World : MonoBehaviour
  {
    public static World instance;

    private MapDataProvider _mapDataProvider;
    private readonly Dictionary<GridPos, List<GridEntity>> _entities = new Dictionary<GridPos, List<GridEntity>>();

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
    public bool IsWalkable(GridPos pos)
    {
      var worldPos = MapUtils.ToWorldPos(pos);
      return !Physics.CheckBox(worldPos, Vector3.one * 0.5f, Quaternion.identity, LayerMask.GetMask("Entities"));
    }

    /// <summary>
    /// Reactive variable showing current floor of the dungeon.
    /// </summary>
    public ReactiveVariable<int> currentFloor = new ReactiveVariable<int>();

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
      _entities.Clear();

      _mapDataProvider = FindObjectOfType<MapDataProvider>();

      if (_mapDataProvider != null)
      {
        _mapDataProvider.Generate();
      }

      currentFloor.CurrentValue++;
    }
  }
}