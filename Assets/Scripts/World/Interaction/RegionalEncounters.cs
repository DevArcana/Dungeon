using System.Collections.Generic;
using EntityLogic;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using UnityEngine.SceneManagement;
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
      _regions = World.instance.AllRegions();
      _enemies = new Dictionary<int, List<GridPos>>();
      PopulateRegions();
      OnTransactionsProcessed();
      
      TurnManager.instance.Transactions.TransactionsProcessed += OnTransactionsProcessed;
      TurnManager.instance.TurnChanged += OnTurnChanged;

      SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
      TurnManager.instance.Transactions.TransactionsProcessed -= OnTransactionsProcessed;
      TurnManager.instance.TurnChanged -= OnTurnChanged;
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
        if (_enemies.ContainsKey(region))
        {
          foreach (var pos in _enemies[region])
          {
            TurnManager.instance.Transactions.EnqueueTransaction(new PanCameraTransaction(false, MapUtils.ToWorldPos(pos), false));
            TurnManager.instance.Transactions.EnqueueTransaction(new SpawnEnemyTransaction(spawnList[_random.Next(spawnList.Count)], pos, false));
          }
          _enemies[region].Clear();
          TurnManager.instance.Transactions.EnqueueTransaction(new PanCameraTransaction(false));
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
        var maxTries = 20;
        while (count < enemyCount)
        {
          maxTries--;
          if (maxTries == 0)
          {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("AAAAAAAAAAAAA");
            return;
          }
          var position = region.cells[_random.Next(region.cells.Count)];

          if (World.instance.GetHeightAt(position) == 0 && !positions.Contains(position) && !World.instance.IsOccupied(position))
          {
            maxTries = 20;
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