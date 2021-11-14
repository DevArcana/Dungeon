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

    public override IEnumerable<GridPos> GetValidTargetPositions()
    {
      var turnManager = TurnManager.instance;
      var allTiles = turnManager.CurrentTurnTaker.GridPos.SquarePattern(7);

      var tilesWithAllies = allTiles.Where(x =>
      {
        var occupant = World.World.instance.GetOccupant(x);
        return occupant is PlayerEntity && occupant != turnManager.CurrentTurnTaker;
      });

      return tilesWithAllies;
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return new[] { pos };
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      return 2;
    }

    public override bool CanExecute(GridPos pos)
    {
      // TODO
      return true;
    }

    public override void Execute(GridPos pos)
    {
      var ally = World.World.instance.GetOccupant(pos);
      TurnManager.instance.Transactions.EnqueueTransaction(new HealAllyTransaction(TurnManager.instance.CurrentTurnTaker, ally, healAmount));
    }
  }
}