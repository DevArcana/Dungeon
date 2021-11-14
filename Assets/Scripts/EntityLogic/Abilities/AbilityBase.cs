using System.Collections.Generic;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  public abstract class AbilityBase : ScriptableObject, IAbility
  {
    public string title;
    public string description;
    public Sprite icon;
    public AbilityTag[] tags;
    public int cooldown;
    
    public abstract IEnumerable<GridPos> GetValidTargetPositions();
    public abstract IEnumerable<GridPos> GetEffectiveRange(GridPos pos);
    public abstract int GetEffectiveCost(GridPos pos);
    public abstract int GetMinimumPossibleCost();
    public abstract bool CanExecute(GridPos pos);
    public abstract void Execute(GridPos pos);
  }
}