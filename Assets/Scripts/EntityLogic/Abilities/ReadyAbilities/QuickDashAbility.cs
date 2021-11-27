using System;
using System.Collections.Generic;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities.ReadyAbilities
{
  [CreateAssetMenu(fileName = "QuickDash", menuName = "Abilities/Quick Dash", order = 1)]
  public class QuickDashAbility : AbilityBase
  {
    private const double CostPerTile = 0.5;
    
    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;

      var maxDistance = (int) (turnManager.ActionPoints.ActionPoints / CostPerTile);

      return startingPosition.Value.Cardinal(maxDistance, false, true, true);
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition)
    {
      return new[] { atPosition };
    }

    public override int GetEffectiveCost(GridPos atPosition)
    {
      var turnTakerPos = TurnManager.instance.CurrentTurnTaker.GridPos;

      var distance = Math.Max(Math.Abs(turnTakerPos.x - atPosition.x), Math.Abs(turnTakerPos.y - atPosition.y));
      return (int) Math.Ceiling(distance * CostPerTile);
    }

    public override int GetMinimumPossibleCost()
    {
      return 1;
    }

    public override void Execute(GridPos atPosition)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, atPosition, true));
    }

    public override string GetCostForTooltip()
    {
      return $"{CostPerTile} AP per tile";
    }
  }
}