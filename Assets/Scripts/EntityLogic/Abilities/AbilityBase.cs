using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using World.Common;

namespace EntityLogic.Abilities
{
  public abstract class AbilityBase : ScriptableObject, IAbility
  {
    public string title;
    public string description;
    public Image icon;
    public AbilityTag[] tags;
    public abstract IEnumerable<GridPos> GetValidTargetPositions();
    public abstract IEnumerable<GridPos> GetEffectiveRange(GridPos pos);
    public abstract int GetEffectiveCost(GridPos pos);
    public abstract void Execute(GridPos pos);
  }
}