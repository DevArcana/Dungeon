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
    public float baseHeal;
    public float focusPercentage;

    public float CalculateHeal()
    {
      return baseHeal + focusPercentage / 100 * TurnManager.instance.CurrentTurnTaker.attributes.Focus;
    }
    
    public override string TooltipDescription()
    {
      return $"Restore {CalculateHeal()} health ({baseHeal} + {focusPercentage}% Focus) to yourself.";
    }
    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;

      yield return startingPosition.Value;
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

    public override void Execute(GridPos atPosition)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new HealSelfTransaction(TurnManager.instance.CurrentTurnTaker, CalculateHeal(), true));
    }
  }
}