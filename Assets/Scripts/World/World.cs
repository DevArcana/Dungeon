﻿using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using EntityLogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using World.Common;
using World.Level;

namespace World
{
  [RequireComponent(typeof(LevelProvider))]
  public class World : MonoBehaviour
  {
    public static World instance;

    private LevelProvider _levelProvider;
    private readonly Dictionary<GridPos, List<GridEntity>> _entities = new Dictionary<GridPos, List<GridEntity>>();

    /// <summary>
    /// Gets height of the heightmap at specified coordinates.
    /// </summary>
    /// <param name="pos">position on the grid</param>
    /// <returns>height at given point</returns>
    public byte GetHeightAt(GridPos pos) => _levelProvider.heightmap[pos.x, pos.y];

    public PathNode[,] GetAreaAround(int distance, GridPos pos)
    {
      var width = distance * 2 + 1;
      var area = new PathNode[width, width];
      var (edgeX, edgeY) = (pos.x - distance, pos.y - distance);

      for (var x = 0; x < width; x++)
      {
        for (var y = 0; y < width; y++)
        {
          var currentPos = GridPos.At(x + edgeX, y + edgeY);
          var isWalkable = !GetEntities(currentPos).Any();
          area[x, y] = new PathNode(currentPos.x, currentPos.y, x, y, GetHeightAt(currentPos), isWalkable);
        }
      }
      
      return area;
    }

    /// <summary>
    /// Adds a given entity to the list of entities in a given tile.
    /// </summary>
    /// <param name="entity">entity to register</param>
    /// <exception cref="ApplicationException">thrown if two living entities exist on the same tile</exception>
    public void Register(GridEntity entity)
    {
      var pos = entity.GridPos;
      if (!_entities.ContainsKey(pos))
      {
        _entities[pos] = new List<GridEntity>();
      }

      var entities = _entities[pos];
      
      foreach (var e in entities)
      {
        switch (e)
        {
          case GridLivingEntity _:
            throw new ApplicationException("There is more than one living entity on the same tile!");
          case GridTriggerEntity trigger:
            trigger.OnTrigger(entity);
            break;
        }
      }

      entities.Add(entity);
    }

    /// <summary>
    /// Remove entity from a given tile.
    /// </summary>
    /// <param name="entity">entity to remove</param>
    public void Unregister(GridEntity entity)
    {
      var pos = entity.GridPos;
      var entities = _entities[pos];
      entities.Remove(entity);
      if (entities.Count == 0)
      {
        _entities.Remove(pos);
      }
    }

    /// <summary>
    /// Gets a list of all entities existing on a given tile.
    /// </summary>
    /// <param name="pos">point on the grid</param>
    /// <returns>a list of grid entities</returns>
    public IEnumerable<GridEntity> GetEntities(GridPos pos)
    {
      if (!_entities.ContainsKey(pos))
      {
        return Array.Empty<GridEntity>();
      }

      return _entities[pos];
    }

    public Dictionary<GridPos, List<GridEntity>> GetAllMapEntities()
    {
      return _entities;
    }

    /// <summary>
    /// Reactive variable showing current floor of the dungeon.
    /// </summary>
    public ReactiveVariable<int> currentFloor = new ReactiveVariable<int>();

    private void Start()
    {
      _levelProvider = GetComponent<LevelProvider>();
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

      DontDestroyOnLoad(gameObject);

      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      _entities.Clear();

      if (_levelProvider != null)
      {
        _levelProvider.Generate();
      }

      currentFloor.CurrentValue++;
    }
  }
}