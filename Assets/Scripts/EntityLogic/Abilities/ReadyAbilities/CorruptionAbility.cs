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
    public float baseDamage;
    public float focusPercentage;
    public int castRange;
    public int impactRange;

    public float CalculateDamage()
    {
      return baseDamage + focusPercentage / 100 * TurnManager.instance.CurrentTurnTaker.attributes.Focus;
    }
    
    public override string TooltipDescription()
    {
      return $"Corrupt terrain around a selected tile, dealing {CalculateDamage()} damage ({baseDamage} + {focusPercentage}% Focus) to all enemies standing on terrain.";
    }

    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;

      return startingPosition.Value.Circle(castRange).Walkable();
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition)
    {
      return atPosition.Circle(impactRange).Walkable();
    }

    public override int GetEffectiveCost(GridPos atPosition)
    {
      return 3;
    }

    public override int GetMinimumPossibleCost()
    {
      return 3;
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
      
      turnManager.Transactions.EnqueueTransaction(new CorruptionTransaction(entities, CalculateDamage(), true));
    }
  }
}