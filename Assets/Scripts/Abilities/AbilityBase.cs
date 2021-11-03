using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using World.Common;

namespace Abilities
{
  public abstract class AbilityBase : ScriptableObject
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