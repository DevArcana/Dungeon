using System;
using System.Collections.Generic;
using EntityLogic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnSystem
{
  public class TurnManager : MonoBehaviour
  {
    public static TurnManager instance;
    
    // components
    
    /// <summary>
    /// Holder for all action points related functionalities
    /// </summary>
    public ActionPointsHolder ActionPoints { get; } = new ActionPointsHolder();

    /// <summary>
    /// Holder for all transactions related functionalities
    /// </summary>
    public TransactionProcessor Transactions { get; } = new TransactionProcessor();
    
    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Destroy(gameObject);
        return;
      }

      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      _entities.Clear();
      ActionPoints.ActionPoints = ActionPointsHolder.MaxActionPoints;
      ActionPoints.ReservedActionPoints = 0;
      Transactions.Clear();
    }

    private void Update()
    {
      Transactions.ProcessTransactions();
    }

    private readonly List<GridLivingEntity> _entities = new List<GridLivingEntity>();

    public GridLivingEntity CurrentTurnTaker => _entities.Count > 0 ? _entities[0] : null;

    public class TurnEventArgs : EventArgs
    {
      public GridLivingEntity Entity { get; set; }
    }

    public event EventHandler<TurnEventArgs> TurnEntityAdded;
    public event EventHandler<TurnEventArgs> TurnChanged;

    private void OnTurnEntityAdded(GridLivingEntity entity)
    {
      TurnEntityAdded?.Invoke(this, new TurnEventArgs {Entity = entity});
    }
    
    private void OnTurnChanged(GridLivingEntity entity)
    {
      TurnChanged?.Invoke(this, new TurnEventArgs {Entity = entity});
    }

    public IEnumerable<GridLivingEntity> PeekQueue(int count)
    {
      if (_entities.Count == 0)
      {
        yield break;
      }
      
      for (var i = 0; i < count; i++)
      {
        yield return _entities[i % _entities.Count];
      }
    }

    public IEnumerable<GridLivingEntity> PeekQueue()
    {
      return _entities;
    }

    public void RegisterTurnBasedEntity(GridLivingEntity entity)
    {
      if (_entities.Contains(entity))
      {
        return;
      }
      
      var inserted = false;
      var current = CurrentTurnTaker;
      
      for (var i = 0; i < _entities.Count; i++)
      {
        if (entity.initiative >= _entities[i].initiative)
        {
          inserted = true;
          _entities.Insert(i, entity);
          break;
        }
      }

      if (!inserted)
      {
        _entities.Add(entity);
      }

      if (current != CurrentTurnTaker)
      {
        ActionPoints.ActionPoints = ActionPointsHolder.MaxActionPoints;
        OnTurnChanged(CurrentTurnTaker);
      }

      OnTurnEntityAdded(entity);
    }

    public void NextTurn()
    {
      var current = CurrentTurnTaker;

      if (current != null)
      {
        current.Highlighted(false);
        _entities.Add(current);
        _entities.RemoveAt(0);
        CurrentTurnTaker.Highlighted(true);
      }

      ActionPoints.ActionPoints = ActionPointsHolder.MaxActionPoints;
      OnTurnChanged(CurrentTurnTaker);
    }

    /// <summary>
    /// Unregisters a given entity from the queue.
    /// </summary>
    /// <param name="entity">entity to unregister</param>
    public void UnregisterTurnBasedEntity(GridLivingEntity entity)
    {
      _entities.Remove(entity);
    }
  }
}
