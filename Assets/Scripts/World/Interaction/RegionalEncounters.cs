using System;
using System.Collections.Generic;
using EntityLogic;
using TurnSystem;
using UnityEngine;
using Utils;
using World.Common;
using Random = System.Random;

namespace World.Interaction
{
  public class RegionalEncounters : MonoBehaviour
  {
    public List<GridLivingEntity> spawnList;
    
    private int _currentRegion;
    private Dictionary<int, Region> _regions;
    private Dictionary<int, List<GridLivingEntity>> _enemies;
    private Random _random;

    private void Start()
    {
      _currentRegion = -1;
      _random = new Random();
      TurnManager.instance.ActionPoints.ActionPointsChanged += OnActionPointsChanged;
      _regions = World.instance.AllRegions();
      _enemies = new Dictionary<int, List<GridLivingEntity>>();
      PopulateRegions();
    }

    private void OnDestroy()
    {
      TurnManager.instance.ActionPoints.ActionPointsChanged -= OnActionPointsChanged;
    }

    private void OnActionPointsChanged(int points)
    {
      var entity = TurnManager.instance.CurrentTurnTaker;

      if (entity == null || !(entity is PlayerEntity))
      {
        return;
      }

      var region = World.instance.GetRegionIndex(entity.GridPos);

      if (region != _currentRegion)
      {
        _currentRegion = region;
        foreach (var enemy in _enemies[region])
        {
          enemy.gameObject.SetActive(true);
          TurnManager.instance.RegisterTurnBasedEntity(enemy);
        }
      }
    }

    private void PopulateRegions()
    {
      // for now, let's hardcode the number of enemies per region
      // each region will get exactly 3 enemies, because why not

      var enemyCount = 3;
      var positions = new HashSet<GridPos>();
      
      foreach (var region in _regions.Values)
      {
        positions.Clear();
        var count = 0;
        while (count < enemyCount)
        {
          var position = region.cells[_random.Next(region.cells.Count)];

          if (!positions.Contains(position) && !World.instance.IsOccupied(position))
          {
            positions.Add(position);
            count++;
            if (!_enemies.ContainsKey(region.index))
            {
              _enemies[region.index] = new List<GridLivingEntity>();
            }
            
            var enemy = Instantiate(spawnList[_random.Next(spawnList.Count)], MapUtils.ToWorldPos(position), Quaternion.identity);
            enemy.gameObject.SetActive(false);
            _enemies[region.index].Add(enemy);
          }
        }
      }
    }
  }
}