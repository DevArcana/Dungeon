using System.Collections.Generic;
using System.Linq;
using EntityLogic.AI;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  [CreateAssetMenu(fileName = "Implicit", menuName = "Abilities/Implicit", order = 1)]
  public class ImplicitAbility : AbilityBase
  {
    public override string TooltipDescription()
    {
      return "";
    }

    public override string TooltipCost()
    {
      return "";
    }

    public override IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      
      var maxCost = TurnManager.instance.ActionPoints.ActionPoints;
      var world = World.World.instance;

      var tiles = turnTaker.pathTree;
      if (tiles == null)
      {
        InfluenceMap.instance.AddEntityInfluence(turnTaker);
        tiles = turnTaker.pathTree;
      }
      var filteredTiles = new List<GridPos>();
      foreach (var element in tiles)
      {
        var (currentPos, tile) = (element.Key, element.Value);
        if (currentPos == turnTaker.GridPos) continue;
        var occupant = world.GetOccupant(currentPos);
        if (occupant is EnemyEntity && turnTaker is PlayerEntity ||
            occupant is PlayerEntity && turnTaker is EnemyEntity)
        {
          if (tile.gCost + 1 <= maxCost)
          {
            filteredTiles.Add(currentPos);
            continue;
          }
        }
      
        if (!(occupant is null)) continue;
        filteredTiles.Add(currentPos);
      }
      
      return filteredTiles;
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition)
    {
      // var pathfinding = new Pathfinding();
      // return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, atPosition).Item1;
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      return Pathfinding.GetPath(turnTaker.pathTree[atPosition]).Select(node => GridPos.At(node.x, node.y));
    }

    public override int GetEffectiveCost(GridPos atPosition)
    {
      // var pathfinding = new Pathfinding();
      // return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, atPosition).Item2 + (World.World.instance.IsOccupied(atPosition) ? 1 : 0);
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      return (int) turnTaker.pathTree[atPosition].gCost + (World.World.instance.IsOccupied(atPosition) ? 1 : 0);
    }

    public override int GetMinimumPossibleCost()
    {
      return 1;
    }

    public override void Execute(GridPos atPosition)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var occupant = World.World.instance.GetOccupant(atPosition);
      
      var path = Pathfinding.GetPath(turnTaker.pathTree[atPosition]).Select(node => GridPos.At(node.x, node.y)).ToList();

      foreach (var segment in path.Take(path.Count - 1))
      {
        turnManager.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, segment, true));
      }
      
      if (occupant is EnemyEntity && turnTaker is PlayerEntity || occupant is PlayerEntity && turnTaker is EnemyEntity)
      {
        turnManager.Transactions.EnqueueTransaction(new AttackTransaction(turnTaker, occupant, true));
      }
      else
      {
        turnManager.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, path.Last(), true));
      }
    }
  }
}