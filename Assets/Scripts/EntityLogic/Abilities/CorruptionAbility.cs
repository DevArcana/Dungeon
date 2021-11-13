using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  [CreateAssetMenu(fileName = "Corruption", menuName = "Abilities/Corruption", order = 1)]
  public class CorruptionAbility : AbilityBase
  {
    public int damage;

    public override IEnumerable<GridPos> GetValidTargetPositions()
    {
      var turnManager = TurnManager.instance;
      return turnManager.CurrentTurnTaker.GridPos.CirclePattern(7);
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return pos.CirclePattern(2);
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      return 3;
    }

    public override void Execute(GridPos pos)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var world = World.World.instance;

      var entities = GetEffectiveRange(pos).Where(x =>
      {
        var occupant = world.GetOccupant(x);
        return !(occupant is null) && AreEnemies(turnTaker, occupant);
      }).Select(x => world.GetOccupant(x));
      
      turnManager.Transactions.EnqueueTransaction(new CorruptionTransaction(turnTaker, entities, damage));
    }

    private static bool AreEnemies(GridLivingEntity first, GridLivingEntity second)
    {
      return first is EnemyEntity && second is PlayerEntity || first is PlayerEntity && second is EnemyEntity;
    }
  }
}