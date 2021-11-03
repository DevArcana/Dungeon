using System;
using System.Collections.Generic;
using EntityLogic;
using Transactions;
using World.Common;

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
    
    public abstract IEnumerable<GridPos> GetValidTargetPositions();
    public abstract IEnumerable<GridPos> GetEffectiveRange(GridPos pos);
    public abstract int GetEffectiveCost(GridPos pos);
    public abstract void Execute(GridPos pos);
  }
}