using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities.ReadyAbilities
{
  [CreateAssetMenu(fileName = "Corruption", menuName = "Abilities/Corruption", order = 1)]
  public class CorruptionAbility : AbilityBase
  {
    public int damage;

    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;
      
      return startingPosition.Value.CirclePattern(7).Walkable();
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition)
    {
      return atPosition.CirclePattern(2).Walkable();
    }

    public override int GetEffectiveCost(GridPos atPosition)
    {
      return 3;
    }

    public override int GetMinimumPossibleCost()
    {
      return 3;
    }

    public override bool CanExecute(GridPos atPosition, GridPos? startingPosition = null)
    {
      // TODO
      return true;
    }

    public override void Execute(GridPos atPosition)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var world = World.World.instance;

      var entities = GetEffectiveRange(atPosition).Where(x =>
      {
        var occupant = world.GetOccupant(x);
        return !(occupant is null) && AbilityUtilities.AreEnemies(turnTaker, occupant);
      }).Select(x => world.GetOccupant(x));
      
      turnManager.Transactions.EnqueueTransaction(new CorruptionTransaction(turnTaker, entities, damage, true));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}