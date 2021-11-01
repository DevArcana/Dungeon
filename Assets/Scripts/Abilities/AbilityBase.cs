using System;
using System.Collections.Generic;
using EntityLogic;
using Transactions;

namespace Abilities
{
  public abstract class AbilityBase
  {
    private TransactionBase _transaction;
    
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<AbilityTag> Tags { get; set; }
    public int Cost => _transaction.Cost;

    protected AbilityBase(GridLivingEntity owner)
    {
      _transaction = new DoNothingTransaction(owner);

      Title = "Do nothing";
      Description = "Do nothing. Menacingly.";
      Tags = Array.Empty<AbilityTag>();
    }
  }
}