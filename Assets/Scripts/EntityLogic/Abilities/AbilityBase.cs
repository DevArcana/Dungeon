using System.Collections.Generic;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  public abstract class AbilityBase : ScriptableObject
  {
    public string title;
    public Sprite icon;
    public AbilityTag[] tags;
    public int cooldown;
    
    public abstract string TooltipDescription();

    public virtual string TooltipCost()
    {
      return $"{GetMinimumPossibleCost()} AP";
    }

    public abstract IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null);
    public abstract IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition);
    public abstract int GetEffectiveCost(GridPos atPosition);
    public abstract int GetMinimumPossibleCost();
    public abstract void Execute(GridPos atPosition);
  }
}