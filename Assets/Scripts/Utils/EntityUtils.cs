using System.Collections.Generic;
using System.Linq;
using Grid;
using TurnSystem;
using UnityEngine;

namespace Utils
{
  public static class EntityUtils
  {
    public static IEnumerable<GridEntity> FindEntitiesInRadius(Vector3 pos, float radius)
    {
      radius *= radius;
      return Object.FindObjectsOfType<GridEntity>().Where(x => (x.transform.position - pos).sqrMagnitude <= radius);
    }
  }
}