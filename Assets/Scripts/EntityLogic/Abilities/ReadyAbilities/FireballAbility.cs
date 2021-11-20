using System;
using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities.ReadyAbilities
{
  [CreateAssetMenu(fileName = "Fireball", menuName = "Abilities/Fireball", order = 1)]
  public class FireballAbility : AbilityBase
  {
    public override IEnumerable<GridPos> GetValidTargetPositions()
    {
      var turnManager = TurnManager.instance;
      var world = World.World.instance;
      
      var allTiles = turnManager.CurrentTurnTaker.GridPos.CirclePattern(5).Walkable();

      var tilesWithEnemies = allTiles.Where(x =>
      {
        var occupant = world.GetOccupant(x);
        return !(occupant == null) && AbilityUtilities.AreEnemies(occupant, turnManager.CurrentTurnTaker);
      });

      var tilesWithTargetableEnemies = new List<GridPos>();
      foreach (var tile in tilesWithEnemies)
      {
        var occupant = world.GetOccupant(tile);
        var turnTaker = turnManager.CurrentTurnTaker;

        var occupantTile = occupant.GridPos;
        var turnTakerTile = turnTaker.GridPos;
        
        var occupantPosition = new Vector3(occupantTile.x, world.GetHeightAt(occupantTile) + 0.5f, occupantTile.y);
        var turnTakerPosition = new Vector3(turnTakerTile.x, world.GetHeightAt(turnTakerTile) + 0.5f, turnTakerTile.y);
        
        if (Physics.Raycast(turnTakerPosition, occupantPosition - turnTakerPosition, out var hit))
        {
          var entity = hit.transform.GetComponent<GridLivingEntity>();
          if (entity == occupant)
          {
            tilesWithTargetableEnemies.Add(tile);
          }
        }
      }

      return tilesWithTargetableEnemies;
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return new[] { pos };
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      return 2;
    }

    public override int GetMinimumPossibleCost()
    {
      return 2;
    }

    public override bool CanExecute(GridPos pos)
    {
      return true;
    }

    public override void Execute(GridPos pos)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var occupant = World.World.instance.GetOccupant(pos);
      
      TurnManager.instance.Transactions.EnqueueTransaction(new FireballTransaction(turnTaker, occupant, 20, true));
    }

    public override string GetCostForTooltip()
    {
      return GetMinimumPossibleCost().ToString();
    }
  }
}
