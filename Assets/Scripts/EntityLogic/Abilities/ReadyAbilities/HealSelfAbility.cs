using System.Collections.Generic;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities.ReadyAbilities
{
  [CreateAssetMenu(fileName = "HealSelf", menuName = "Abilities/Heal Self", order = 1)]
  public class HealSelfAbility : AbilityBase
  {
    public int healAmount;

    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;
      
      return new[] { startingPosition.Value };
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition)
    {
      return new[] { atPosition };
    }

    public override int GetEffectiveCost(GridPos atPosition)
    {
      return 2;
    }

    public override int GetMinimumPossibleCost()
    {
      return 2;
    }

    public override bool CanExecute(GridPos atPosition, GridPos? startingPosition = null)
    {
      // TODO
      return true;
    }

    public override void Execute(GridPos atPosition)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new HealSelfTransaction(TurnManager.instance.CurrentTurnTaker, healAmount, true));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}