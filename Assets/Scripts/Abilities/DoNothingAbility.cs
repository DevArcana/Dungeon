using System;
using System.Collections.Generic;
using EntityLogic;
using UnityEngine;
using World.Common;

namespace Abilities
{
  [CreateAssetMenu(fileName = "DoNothing", menuName = "Abilities/Do nothing", order = 1)]
  public class DoNothingAbility : AbilityBase
  {
    public override IEnumerable<GridPos> GetValidTargetPositions()
    {
      return Array.Empty<GridPos>();
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return Array.Empty<GridPos>();
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      return 0;
    }

    public override void Execute(GridPos pos)
    {
    }
  }
}