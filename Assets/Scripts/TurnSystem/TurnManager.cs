using System;
using System.Collections.Generic;
using TurnSystem.Transactions;
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
        ActionPoints = 5;
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

      ActionPoints = 5;
      OnTurnChanged(CurrentTurnTaker);
    }

    #region Transactions

    private readonly Queue<TransactionBase> _transactionQueue = new Queue<TransactionBase>();

    /// <summary>
    /// Current actual amount of action points still left.
    /// </summary>
    public int ActionPoints { get; private set; }
    
    /// <summary>
    /// Amount of action points reserved by transactions.
    /// </summary>
    public int ReservedActionPoints { get; private set; }

    /// <summary>
    /// Available unallocated action points still remaining.
    /// </summary>
    public int RemainingActionPoints => ActionPoints - ReservedActionPoints;

    /// <summary>
    /// Indicates whether there are any queued up transactions being processed.
    /// </summary>
    /// <remarks>Can be used to gray out UI or block adding new transactions before previous ones finished.</remarks>
    public bool TransactionPending => _transactionQueue.Count > 0;

    /// <summary>
    /// Checks whether there are enough action points to process the transaction and whether it is the owner's turn (if owner is provided).
    /// </summary>
    /// <param name="transaction">Transaction to be processed.</param>
    /// <returns>Boolean indicating whether processing is possible.</returns>
    public bool CanProcessTransaction(TransactionBase transaction)
    {
      return RemainingActionPoints >= transaction.Cost && (transaction.Owner == CurrentTurnTaker || transaction.Owner == null);
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

      ReservedActionPoints += transaction.Cost;
      _transactionQueue.Enqueue(transaction);
      
      return true;
    }

    private void Update()
    {
      if (_transactionQueue.Count == 0)
      {
        return;
      }
      // process all transactions until none are left
      var transaction = _transactionQueue.Peek();
      if (transaction.Run())
      {
        ReservedActionPoints -= transaction.Cost;
        ActionPoints -= transaction.Cost;
        _transactionQueue.Dequeue();
        
        if (_transactionQueue.Count == 0 && ActionPoints == 0)
        {
          NextTurn();
        }
      }
    }

    #endregion
  }
}
