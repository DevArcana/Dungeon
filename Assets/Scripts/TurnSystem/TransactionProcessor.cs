using System.Collections.Generic;
using TurnSystem.Transactions;

namespace TurnSystem
{
  public class TransactionProcessor
  {
    private TransactionBase _transaction;
    private readonly Queue<TransactionBase> _transactionQueue = new Queue<TransactionBase>();

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
          _transaction = null;
        }
      }
    }
  }
}