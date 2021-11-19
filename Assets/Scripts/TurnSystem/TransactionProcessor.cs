using System;
using System.Collections.Generic;
using TurnSystem.Transactions;

namespace TurnSystem
{
  public class TransactionProcessor
  {
    private TransactionBase _transaction;
    private readonly Queue<TransactionBase> _transactionQueue = new Queue<TransactionBase>();

    public event Action TransactionsProcessed;
    public event Action AbilityTransactionsProcessed;
    private bool _justFinished = false;

    /// <summary>
    /// Indicates whether there exists a pending transaction.
    /// </summary>
    public bool HasPendingTransactions => _transaction != null || _transactionQueue.Count > 0;
    
    /// <summary>
    /// Pushes the transaction to the queue of pending transactions.
    /// </summary>
    /// <param name="transaction">Transaction to be queued and processed.</param>
    /// <remarks>Also checks whether the transaction can be performed.</remarks>
    public void EnqueueTransaction(TransactionBase transaction)
    { 
      _transactionQueue.Enqueue(transaction);
    }

    public void Clear()
    {
      _transaction = null;
      _transactionQueue.Clear();
    }

    /// <summary>
    /// Called each frame.
    /// </summary>
    public void ProcessTransactions()
    {
      if (_transaction == null && _transactionQueue.Count > 0)
      {
        _transaction = _transactionQueue.Dequeue();
      }

      if (_transaction != null)
      {
        if (_transaction.Run())
        {
          // a temporary hack to make abilities work again
          if (_transaction.IsAbility)
          {
            if (_transactionQueue.Count == 0 || _transactionQueue.Count > 0 && !_transactionQueue.Peek().IsAbility)
            {
              AbilityTransactionsProcessed?.Invoke();
            }
          }
          _transaction = null;

          if (_transactionQueue.Count == 0)
          {
            _justFinished = true;
          }
        }
      }
      else if (_justFinished)
      {
        TransactionsProcessed?.Invoke();
        _justFinished = false;
      }
    }
  }
}