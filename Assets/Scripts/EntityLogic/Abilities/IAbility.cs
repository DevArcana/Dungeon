using System.Collections.Generic;
using World.Common;

namespace EntityLogic.Abilities
{
  public interface IAbility
  {
    IEnumerable<GridPos> GetValidTargetPositions();
    IEnumerable<GridPos> GetEffectiveRange(GridPos pos);
    int GetEffectiveCost(GridPos pos);
    void Execute(GridPos pos);
  }
}