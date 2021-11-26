using System.Collections.Generic;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities.ReadyAbilities
{
  [CreateAssetMenu(fileName = "HealAlly", menuName = "Abilities/Heal Ally", order = 1)]
  public class HealAllyAbility : AbilityBase
  {
    public int healAmount;

    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;

      var turnTaker = turnManager.CurrentTurnTaker;

      return startingPosition.Value.Circle(7, false).OccupiedByAlliesOf(turnTaker).Wounded();
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
      var ally = World.World.instance.GetOccupant(atPosition);
      TurnManager.instance.Transactions.EnqueueTransaction(new HealAllyTransaction(TurnManager.instance.CurrentTurnTaker, ally, healAmount, true));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}