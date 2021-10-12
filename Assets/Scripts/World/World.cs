using System;
using System.Collections.Generic;
using UnityEngine;
using World.Common;
using World.Entities;
using World.Level;

namespace World
{
  [RequireComponent(typeof(LevelProvider))]
  public class World : MonoBehaviour
  {
    public static World instance;

    private LevelProvider _levelProvider;
    private readonly Dictionary<GridPos, List<GridEntity>> _entities = new Dictionary<GridPos, List<GridEntity>>();

    public byte GetHeightAt(GridPos pos) => _levelProvider.heightmap[pos.x, pos.y];

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
        switch (e.CollisionType)
        {
          case EntityCollisionType.Solid:
            throw new ApplicationException("Multiple solid entities on the same tile!");
          case EntityCollisionType.Trigger:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      entities.Add(entity);
    }

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

    public IEnumerable<GridEntity> GetEntities(GridPos pos)
    {
      if (!_entities.ContainsKey(pos))
      {
        return Array.Empty<GridEntity>();
      }

      return _entities[pos];
    }

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
      }

      DontDestroyOnLoad(gameObject);
    }
  }
}