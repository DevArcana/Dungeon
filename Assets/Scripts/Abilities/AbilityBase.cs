using System;
using System.Collections.Generic;
using EntityLogic;
using Transactions;
using UnityEngine;
using UnityEngine.UI;
using World.Common;

namespace Abilities
{
  public abstract class AbilityBase : ScriptableObject
  {
    private TransactionBase _transaction;

    public string title;
    public string description;
    public Image icon;
    public AbilityTag[] tags;
    
    public int Cost => _transaction.Cost;

    protected AbilityBase(GridLivingEntity owner)
    {
      _transaction = new DoNothingTransaction(owner);
      title = "Do nothing";
      description = "Do nothing. Menacingly.";
      tags = Array.Empty<AbilityTag>();
    }
    
    public abstract IEnumerable<GridPos> GetValidTargetPositions();
    public abstract IEnumerable<GridPos> GetEffectiveRange(GridPos pos);
    public abstract int GetEffectiveCost(GridPos pos);
    public abstract void Execute(GridPos pos);
  }
}