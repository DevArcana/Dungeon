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
    public float baseCostPerTile;
    public float reductionPerAgilityPoint;

    public float CalculateCostPerTile()
    {
      return Math.Max(0.25f, baseCostPerTile - reductionPerAgilityPoint * TurnManager.instance.CurrentTurnTaker.attributes.Agility);
    }
    
    public override string TooltipDescription()
    {
      return $"Dash in a cardinal direction. Cannot dash through units or obstacles. Cost per tile is lowered by {reductionPerAgilityPoint} per 1 Agility point.";
    }

    public override string TooltipCost()
    {
      return $"{CalculateCostPerTile()} AP per tile";
    }

    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;

      var maxDistance = (int) (turnManager.ActionPoints.ActionPoints / CalculateCostPerTile());

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
      return (int) Math.Ceiling(distance * CalculateCostPerTile());
    }

    public override int GetMinimumPossibleCost()
    {
      return 1;
    }

    public override void Execute(GridPos atPosition)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, atPosition, true));
    }
  }
}