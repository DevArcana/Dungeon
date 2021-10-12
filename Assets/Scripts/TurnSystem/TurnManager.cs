using System;
using System.Collections.Generic;
using Transactions;
using UnityEngine;
using World.Entities;

namespace TurnSystem
{
  public class TurnManager : MonoBehaviour
  {
    public static TurnManager instance;
    
    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Destroy(gameObject);
      }
      
      DontDestroyOnLoad(gameObject);
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

    public void RegisterTurnBasedEntity(GridLivingEntity entity)
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

    #region Transactions

    private TransactionBase _transaction;
    private readonly Queue<TransactionBase> _transactionQueue = new Queue<TransactionBase>();

    /// <summary>
    /// Holder for all action points related functionalities
    /// </summary>
    public ActionPointsHolder ActionPoints { get; } = new ActionPointsHolder();

    /// <summary>
    /// Indicates whether there are any queued up transactions being processed.
    /// </summary>
    /// <remarks>Can be used to gray out UI or block adding new transactions before previous ones finished.</remarks>
    public bool TransactionPending => _transaction != null;

    /// <summary>
    /// Checks whether there are enough action points to process the transaction and whether it is the owner's turn (if owner is provided).
    /// </summary>
    /// <param name="transaction">Transaction to be processed.</param>
    /// <returns>Boolean indicating whether processing is possible.</returns>
    public bool CanProcessTransaction(TransactionBase transaction)
    {
      return transaction.CanExecute() && ActionPoints.RemainingActionPoints >= transaction.Cost && (transaction.Owner == CurrentTurnTaker || transaction.Owner == null);
    }

    /// <summary>
    /// Pushes the transaction to the queue of pending transactions.
    /// </summary>
    /// <param name="transaction">Transaction to be queued and processed.</param>
    /// <returns>Boolean indicating whether transaction was put in the queue.</returns>
    /// <remarks>Also checks whether the transaction can be performed.</remarks>
    public bool EnqueueTransaction(TransactionBase transaction)
    {
      if (!CanProcessTransaction(transaction))
      {
        return false;
      }

      ActionPoints.ReservedActionPoints += transaction.Cost;
      _transactionQueue.Enqueue(transaction);
      
      return true;
    }

    private void Update()
    {
      if (_transaction == null && _transactionQueue.Count > 0)
      {
        _transaction = _transactionQueue.Dequeue();
        if (!_transaction.CanExecute())
        {
          ActionPoints.ReservedActionPoints -= _transaction.Cost;
          _transaction = null;
        }
      }

      if (_transaction != null)
      {
        if (_transaction.Run())
        {
          ActionPoints.ActionPoints -= _transaction.Cost;
          ActionPoints.ReservedActionPoints -= _transaction.Cost;
          _transaction = null;

          if (_transactionQueue.Count == 0 && ActionPoints.ActionPoints == 0)
          {
            NextTurn();
          }
        }
      }
    }

    #endregion
  }
}
