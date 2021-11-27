using System.Collections.Generic;
using System.Linq;
using EntityLogic;
using EntityLogic.Abilities;

namespace World.Common
{
  public static class GridPosExtensions
  {
    public static IEnumerable<GridPos> Square(this GridPos startingTile, int radius, bool includeStartingTile = true)
    {
      for (var y = startingTile.y - radius; y <= startingTile.y + radius; y++)
      {
        for (var x = startingTile.x - radius; x <= startingTile.x + radius; x++)
        {
          var tile = GridPos.At(x, y);

          if (!includeStartingTile && tile == startingTile)
          {
            continue;
          }

          yield return tile;
        }
      }
    }

    public static IEnumerable<GridPos> Circle(this GridPos startingTile, int radius, bool includeStartingTile = true)
    {
      var boundingBox = startingTile.Square(radius, includeStartingTile).ToArray();

      foreach (var tile in boundingBox)
      {
        if (!includeStartingTile && tile == startingTile)
        {
          continue;
        }
        
        var distance = tile.TwoDimDistance(startingTile);

        if (distance <= radius + 0.5)
        {
          yield return tile;
        }
      }
    }

    public static IEnumerable<GridPos> Cardinal(this GridPos startingTile, int radius, bool includeStartingTile = true, bool stopOnEnemies = false, bool stopOnWalls = false)
    {
      var world = World.instance;
      
      if (includeStartingTile)
      {
        yield return startingTile;
      }

      var currentTile = startingTile;
      var startingHeight = world.GetHeightAt(startingTile);

      GridPos Next(int directionNumber) =>
        directionNumber switch
        {
          0 => currentTile.North,
          1 => currentTile.East,
          2 => currentTile.South,
          3 => currentTile.West,
          _ => GridPos.At(-1, -1) // won't ever reach this
        };

      for (var i = 0; i < 4; i++)
      {
        currentTile = startingTile;
        for (var j = 0; j < radius; j++)
        {
          currentTile = Next(i);
          if (world.IsOccupied(currentTile) && stopOnEnemies)
          {
            break;
          }

          if (world.IsWalkable(currentTile) && world.GetHeightAt(currentTile) == startingHeight)
          {
            yield return currentTile;
          }
          else if (stopOnWalls)
          {
            break;
          }
        }
      }
    }

    public static IEnumerable<GridPos> Walkable(this IEnumerable<GridPos> tiles)
    {
      var world = World.instance;
      return tiles.Where(x => world.IsWalkable(x));
    }

    public static IEnumerable<GridPos> OccupiedByAlliesOf(this IEnumerable<GridPos> tiles, GridLivingEntity entity, bool includeSelf = false)
    {
      foreach (var tile in tiles)
      {
        var occupant = World.instance.GetOccupant(tile);

        if (occupant == null)
        {
          continue;
        }

        if (AbilityUtilities.AreEnemies(occupant, entity))
        {
          continue;
        }

        if (occupant != entity || includeSelf && occupant == entity)
        {
          yield return tile;
        }
      }
    }

    public static IEnumerable<GridPos> OccupiedByEnemiesOf(this IEnumerable<GridPos> tiles, GridLivingEntity entity)
    {
      foreach (var tile in tiles)
      {
        var occupant = World.instance.GetOccupant(tile);

        if (occupant == null || !AbilityUtilities.AreEnemies(occupant, entity))
        {
          continue;
        }

        yield return tile;
      }
    }

    public static IEnumerable<GridPos> Wounded(this IEnumerable<GridPos> tiles)
    {
      foreach (var tile in tiles)
      {
        var occupantHealth = World.instance.GetOccupant(tile)?.health;

        if (occupantHealth != null && occupantHealth.Health != occupantHealth.MaximumHealth)
        {
          yield return tile;
        }
      }
    }

    public static IEnumerable<GridPos> VisibleFrom(this IEnumerable<GridPos> tiles, GridPos fromTile)
    {
      foreach (var tile in tiles)
      {
        var occupant = World.instance.GetOccupant(tile);

        if (occupant != null && AbilityUtilities.InSight(occupant, fromTile))
        {
          yield return tile;
        }
      }
    }
  }
}