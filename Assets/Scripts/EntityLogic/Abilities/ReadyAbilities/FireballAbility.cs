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
    public float baseDamage;
    public float focusPercentage;
    public int castRange;

    public float CalculateDamage()
    {
      return baseDamage + focusPercentage / 100 * TurnManager.instance.CurrentTurnTaker.attributes.Focus;
    }
    
    public override string TooltipDescription()
    {
      return $"Throw fireball at an enemy, dealing {CalculateDamage()} damage ({baseDamage} + {focusPercentage}% Focus).";
    }
    
    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      startingPosition ??= turnManager.CurrentTurnTaker.GridPos;
      
      var turnTaker = turnManager.CurrentTurnTaker;
      
      return startingPosition.Value.Circle(castRange).OccupiedByEnemiesOf(turnTaker).VisibleFrom(turnTaker.GridPos);
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
      
      TurnManager.instance.Transactions.EnqueueTransaction(new FireballTransaction(occupant, CalculateDamage(), true));
    }
  }
}
