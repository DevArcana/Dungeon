﻿using System.Collections.Generic;
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

    public override int GetMinimumPossibleCost()
    {
      return 3;
    }

    public override bool CanExecute(GridPos pos)
    {
      // TODO
      return true;
    }

    public override void Execute(GridPos pos)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var world = World.World.instance;

      var entities = GetEffectiveRange(pos).Where(x =>
      {
        var occupant = world.GetOccupant(x);
        return !(occupant is null) && AbilityUtilities.AreEnemies(turnTaker, occupant);
      }).Select(x => world.GetOccupant(x));
      
      turnManager.Transactions.EnqueueTransaction(new CorruptionTransaction(turnTaker, entities, damage));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}