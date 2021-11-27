using System.Collections.Generic;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities.ReadyAbilities
{
  [CreateAssetMenu(fileName = "Fireball", menuName = "Abilities/Fireball", order = 1)]
  public class FireballAbility : AbilityBase
  {
    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;
      
      var turnTaker = turnManager.CurrentTurnTaker;
      
      return startingPosition.Value.Circle(5).OccupiedByEnemiesOf(turnTaker).VisibleFrom(turnTaker.GridPos);
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
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var occupant = World.World.instance.GetOccupant(atPosition);
      
      TurnManager.instance.Transactions.EnqueueTransaction(new FireballTransaction(turnTaker, occupant, 20, true));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}
