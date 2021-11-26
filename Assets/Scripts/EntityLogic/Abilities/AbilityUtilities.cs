using TurnSystem;
using UnityEngine;
using Utils;
using World.Common;

namespace EntityLogic.Abilities
{
  public static class AbilityUtilities
  {
    public static bool AreEnemies(GridLivingEntity first, GridLivingEntity second)
    {
      return first is EnemyEntity && second is PlayerEntity || first is PlayerEntity && second is EnemyEntity;
    }

    public static bool InSight(GridLivingEntity entity, GridPos fromTile)
    {
      var entityTile = entity.GridPos;

      var tilePosition = MapUtils.ToWorldPos(fromTile) + new Vector3(0, 0.5f, 0);
      var entityPosition = MapUtils.ToWorldPos(entityTile) + new Vector3(0, 0.5f, 0);
      var direction = entityPosition - tilePosition;
        
      if (Physics.Raycast(tilePosition, direction, out var hit, maxDistance: direction.magnitude))
      {
        var entityHit = hit.transform.GetComponent<GridLivingEntity>();

        return entityHit != null && entityHit == entity;
      }
      else
      {
        return false;
      }
    }
  }
}