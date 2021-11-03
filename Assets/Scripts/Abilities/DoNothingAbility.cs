using System;
using System.Collections.Generic;
using EntityLogic;
using World.Common;

namespace Abilities
{
  public class DoNothingAbility : AbilityBase
  {
    public DoNothingAbility(GridLivingEntity owner) : base(owner)
    {
      
    }

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