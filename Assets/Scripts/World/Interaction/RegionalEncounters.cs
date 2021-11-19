using System.Collections.Generic;
using EntityLogic;
using TurnSystem;
using TurnSystem.Transactions;
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
    private Dictionary<int, List<GridPos>> _enemies;
    private Random _random;

    private void Start()
    {
      _currentRegion = -1;
      _random = new Random();
      TurnManager.instance.Transactions.TransactionsProcessed += OnTransactionsProcessed;
      TurnManager.instance.TurnChanged += OnTurnChanged;
      _regions = World.instance.AllRegions();
      _enemies = new Dictionary<int, List<GridPos>>();
      PopulateRegions();
      OnTransactionsProcessed();
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      OnTransactionsProcessed();
    }

    private void OnDestroy()
    {
      TurnManager.instance.Transactions.TransactionsProcessed -= OnTransactionsProcessed;
      TurnManager.instance.TurnChanged -= OnTurnChanged;
    }

    private void OnTransactionsProcessed()
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
        foreach (var pos in _enemies[region])
        {
          TurnManager.instance.Transactions.EnqueueTransaction(new PanCameraTransaction(false, MapUtils.ToWorldPos(pos), false));
          TurnManager.instance.Transactions.EnqueueTransaction(new SpawnEnemyTransaction(spawnList[_random.Next(spawnList.Count)], pos, false));
        }
        _enemies[region].Clear();
        TurnManager.instance.Transactions.EnqueueTransaction(new PanCameraTransaction(false));
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
              _enemies[region.index] = new List<GridPos>();
            }
            _enemies[region.index].Add(position);
          }
        }
      }
    }
  }
}