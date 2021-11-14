using System;
using System.Collections.Generic;
using System.Linq;
using EntityLogic.AI;
using TurnSystem;
using TurnSystem.Transactions;
using World.Common;

namespace EntityLogic.Abilities
{
  public class ImplicitAbility : IAbility
  {
    public IEnumerable<GridPos> GetValidTargetPositions()
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      var pos = turnTaker.GridPos;
      var maxCost = TurnManager.instance.ActionPoints.ActionPoints;
      var world = World.World.instance;
      
      var pathFinding = new Pathfinding();
      var tiles = pathFinding.GetShortestPathTree(pos, maxCost);
      var filteredTiles = new List<GridPos>();
      foreach (var tile in tiles)
      {
        var currentPos = GridPos.At(tile.x, tile.y);
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

    public IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      var pathfinding = new Pathfinding();
      return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, pos).Item1;
    }

    public int GetEffectiveCost(GridPos pos)
    {
      var pathfinding = new Pathfinding();
      return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, pos).Item2 + (World.World.instance.IsOccupied(pos) ? 1 : 0);
    }

    public bool CanExecute(GridPos pos)
    {
      return true;
    }

    public void Execute(GridPos pos)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var occupant = World.World.instance.GetOccupant(pos);
      
      var pathfinding = new Pathfinding();
      var path = pathfinding.FindPath(turnTaker.GridPos, pos).Item1;

      foreach (var segment in path.Take(path.Count - 1))
      {
        turnManager.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, segment));
      }
      
      if (occupant is EnemyEntity && turnTaker is PlayerEntity || occupant is PlayerEntity && turnTaker is EnemyEntity)
      {
        turnManager.Transactions.EnqueueTransaction(new AttackTransaction(turnTaker, occupant, 10));
      }
      else
      {
        turnManager.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, path.Last()));
      }
    }
  }
}