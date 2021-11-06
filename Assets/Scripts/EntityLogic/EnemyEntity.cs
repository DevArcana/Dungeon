using System.IO;
using EntityLogic.AI;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic
{
  public class EnemyEntity : GridLivingEntity
  {
    private void Update()
    {
      if (TurnManager.instance.CurrentTurnTaker == this && !TurnManager.instance.Transactions.HasPendingTransactions)
      {
        var player = Pathfinding.FindClosestPlayer(GridPos);
        var pathfinding = new Pathfinding();
        var ability = abilities.SelectedAbility;

        var bestCost = int.MaxValue;
        GridPos? target = null;
        foreach (var targetPosition in ability.GetValidTargetPositions())
        {
          var cost = pathfinding.FindPath(targetPosition, player.GridPos).Item2;
          if (player.GridPos == targetPosition)
          {
            target = targetPosition;
            break;
          }

          if (cost < bestCost)
          {
            bestCost = cost;
            target = targetPosition;
          }
        }

        if (!target.HasValue)
        {
          Debug.LogError("WTF");
        }
        else
        {
          TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(target.Value));
          TurnManager.instance.ActionPoints.SpendReservedPoints();
          ability.Execute(target.Value);
        }

        return;
        var utilityAI = new UtilityAI();
        utilityAI.PerformNextAction(this);
      }
    }
    
    // unity components
    private DamageableEntity _damageable;

    protected override void Start()
    {
      base.Start();
      _damageable = GetComponent<DamageableEntity>();
    }

    public override string GetTooltip()
    {
      return $"HP: {_damageable.damageable.Health}/{_damageable.damageable.MaxHealth}";
    }
  }
}