using System;
using EntityLogic;
using EntityLogic.AI;
using UnityEngine;

namespace TurnSystem.Transactions
{
  public class HealSelfTransaction : TransactionBase
  {
    private readonly GridLivingEntity _entity;
    private readonly float _healAmount;

    public HealSelfTransaction(GridLivingEntity entity, float healAmount, bool isAbility) : base(isAbility)
    {
      _entity = entity;
      _healAmount = healAmount;
    }

    protected override void Process()
    {
      var entityHealth = _entity.health;
      var startHealthAmount = entityHealth.Health;
      entityHealth?.Heal(_healAmount);
      if (_entity is PlayerEntity)
      {
        LogConsole.Log($"Healed from {Mathf.Ceil(startHealthAmount)} to {Mathf.Ceil(_entity.health.Health)}!" + Environment.NewLine);
      }
      Finish();
    }
  }
}