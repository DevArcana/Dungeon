using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnSystem
{
  public class TurnManager : MonoBehaviour
  {
    public static TurnManager Instance { get; private set; }
    
    private void Awake()
    {
      if (Instance != null)
      {
        Debug.LogWarning("Multiple turn managers in scene!");
      }

      Instance = this;
      DontDestroyOnLoad(gameObject);
    }

    private readonly List<TurnBasedEntity> _entities = new List<TurnBasedEntity>();

    public TurnBasedEntity CurrentTurnTaker => _entities.Count > 0 ? _entities[0] : null;

    public class TurnEventArgs : EventArgs
    {
      public TurnBasedEntity Entity { get; set; }
    }

    public event EventHandler<TurnEventArgs> TurnEntityAdded;
    public event EventHandler<TurnEventArgs> TurnChanged;

    private void OnTurnEntityAdded(TurnBasedEntity entity)
    {
      TurnEntityAdded?.Invoke(this, new TurnEventArgs {Entity = entity});
    }
    
    private void OnTurnChanged(TurnBasedEntity entity)
    {
      TurnChanged?.Invoke(this, new TurnEventArgs {Entity = entity});
    }

    public IEnumerable<TurnBasedEntity> PeekQueue(int count)
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

    public void RegisterTurnBasedEntity(TurnBasedEntity entity)
    {
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
      
      OnTurnChanged(CurrentTurnTaker);
    }
  }
}
