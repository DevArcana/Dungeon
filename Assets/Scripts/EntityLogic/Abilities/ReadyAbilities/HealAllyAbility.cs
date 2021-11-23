using System.Collections.Generic;
using System.Linq;
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
      
      var allTiles = startingPosition.Value.SquarePattern(7).Walkable();

      var tilesWithAllies = allTiles.Where(x =>
      {
        var occupant = World.World.instance.GetOccupant(x);
        return occupant is PlayerEntity && occupant != turnManager.CurrentTurnTaker;
      });

      return tilesWithAllies;
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
      var ally = World.World.instance.GetOccupant(atPosition);
      TurnManager.instance.Transactions.EnqueueTransaction(new HealAllyTransaction(TurnManager.instance.CurrentTurnTaker, ally, healAmount, true));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}