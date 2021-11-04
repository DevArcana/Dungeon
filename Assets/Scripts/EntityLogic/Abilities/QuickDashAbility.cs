using System;
using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  [CreateAssetMenu(fileName = "QuickDash", menuName = "Abilities/Quick Dash", order = 1)]
  public class QuickDashAbility : AbilityBase
  {
    private const double CostPerTile = 0.4;
    
    public override IEnumerable<GridPos> GetValidTargetPositions()
    {
      var turnManager = TurnManager.instance;
      return turnManager.CurrentTurnTaker.GridPos.Pattern(Pattern.Cardinal, (int)(turnManager.ActionPoints.ActionPoints / CostPerTile), true, false);
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return new[] { pos };
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      var turnTakerPos = TurnManager.instance.CurrentTurnTaker.GridPos;

      var distance = Math.Max(Math.Abs(turnTakerPos.x - pos.x), Math.Abs(turnTakerPos.y - pos.y));
      return (int) Math.Ceiling(distance * CostPerTile);
    }

    public override void Execute(GridPos pos)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, pos));
    }
  }
}