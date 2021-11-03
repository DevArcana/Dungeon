using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using TurnSystem;
using World.Common;

namespace Abilities
{
  public class MoveAbility : AbilityBase
  {
    public override IEnumerable<GridPos> GetValidTargetPositions()
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

              if (!world.IsWalkable(pos)) continue;

              if (neighbour.cost <= TurnManager.instance.ActionPoints.ActionPoints)
              {
                tiles[pos] = neighbour;
                queue.Enqueue(neighbour.gridPos);
              }
            }
          }
        }
      }
      return tiles.Select(x => x.Value.gridPos);
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return new[] { pos };
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      var pathfinding = new Pathfinding();
      return pathfinding.FindPath(TurnManager.instance.CurrentTurnTaker.GridPos, pos).Item2;
    }

    public override void Execute(GridPos pos)
    {
    }
  }
}