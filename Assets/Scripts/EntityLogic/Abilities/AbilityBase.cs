using System.Collections.Generic;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  public abstract class AbilityBase : ScriptableObject
  {
    public string title;
    public string description;
    public Sprite icon;
    public AbilityTag[] tags;
    public int cooldown;
    
    public abstract IEnumerable<GridPos> GetValidTargetPositions(GridPos? startingPosition = null);
    public abstract IEnumerable<GridPos> GetEffectiveRange(GridPos atPosition);
    public abstract int GetEffectiveCost(GridPos atPosition);
    public abstract int GetMinimumPossibleCost();
    public abstract bool CanExecute(GridPos atPosition, GridPos? startingPosition = null);
    public abstract void Execute(GridPos atPosition);
    public abstract string GetCostForTooltip();
  }
}