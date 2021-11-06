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
      var world = World.World.instance;

      var turnTaker = TurnManager.instance.CurrentTurnTaker;

      var pos = turnTaker.GridPos;
      var cost = 0;
      var height = world.GetHeightAt(pos);

      var tiles = new Dictionary<GridPos, Tile>
      {
        [pos] = new Tile(pos, height, cost)
      };

      var queue = new Queue<GridPos>();
      queue.Enqueue(pos);

      while (queue.Any())
      {
        var tile = tiles[queue.Dequeue()];

        for (var x = -1; x <= 1; x++)
        {
          for (var y = -1; y <= 1; y++)
          {
            if (x == 0 && y == 0) continue;

            pos = GridPos.At(x + tile.gridPos.x, y + tile.gridPos.y);
            if (pos == turnTaker.GridPos) continue;

            if (tiles.ContainsKey(pos))
            {
              var neighbour = tiles[pos];
              var heightDifference = Math.Abs(tile.height - neighbour.height);
              if (heightDifference > 1) continue;

              cost = tile.cost + heightDifference + 1;
              if (cost < neighbour.cost)
              {
                neighbour.cost = cost;
                if (neighbour.cost <= TurnManager.instance.ActionPoints.ActionPoints)
                {
                  queue.Enqueue(neighbour.gridPos);
                }
              }
            }
            else
            {
              height = world.GetHeightAt(pos);
              var heightDifference = Math.Abs(tile.height - height);

              if (heightDifference > 1) continue;

              var neighbour = new Tile(pos, height, tile.cost + heightDifference + 1);

              var occupant = world.GetOccupant(pos);
              if (occupant is PlayerEntity) continue;

              if ((occupant == null && neighbour.cost <= TurnManager.instance.ActionPoints.ActionPoints) ||
                  (occupant != null && neighbour.cost + 2 <= TurnManager.instance.ActionPoints.ActionPoints))
              {
                tiles[pos] = neighbour;
                queue.Enqueue(neighbour.gridPos);
              }
            }
          }
        }
      }
      return tiles.Where(x => !(x.Value.gridPos.x == turnTaker.GridPos.x && x.Value.gridPos.y == turnTaker.GridPos.y)).Select(x => x.Value.gridPos);
    }

    public IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      var pathfinding = new Pathfinding();
      return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, pos).Item1;
    }

    public int GetEffectiveCost(GridPos pos)
    {
      var pathfinding = new Pathfinding();
      return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, pos).Item2 + (World.World.instance.IsOccupied(pos) ? 2 : 0);
    }

    public void Execute(GridPos pos)
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;
      var occupant = World.World.instance.GetOccupant(pos);

      if ((occupant is EnemyEntity && turnTaker is PlayerEntity || occupant is PlayerEntity && turnTaker is EnemyEntity) && pos.OneDimDistance(turnTaker.GridPos) == 1)
      {
        turnManager.Transactions.EnqueueTransaction(new AttackTransaction(turnTaker, occupant, 10));
      }
      else
      {
        var pathfinding = new Pathfinding();
        var path = pathfinding.FindPath(turnTaker.GridPos, pos).Item1;

        foreach (var segment in path)
        {
          turnManager.Transactions.EnqueueTransaction(new MoveTransaction(TurnManager.instance.CurrentTurnTaker, segment));
        }
      }
    }
  }
}