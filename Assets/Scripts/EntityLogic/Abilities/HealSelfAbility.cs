using System.Collections.Generic;
using System.Linq;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  [CreateAssetMenu(fileName = "HealSelf", menuName = "Abilities/Heal Self", order = 1)]
  public class HealSelfAbility : AbilityBase
  {
    public int healAmount;

    public override IEnumerable<GridPos> GetValidTargetPositions()
    {
      return new[] { TurnManager.instance.CurrentTurnTaker.GridPos };
    }

    public override IEnumerable<GridPos> GetEffectiveRange(GridPos pos)
    {
      return new[] { pos };
    }

    public override int GetEffectiveCost(GridPos pos)
    {
      return 2;
    }

    public override void Execute(GridPos pos)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new HealSelfTransaction(TurnManager.instance.CurrentTurnTaker, healAmount));
    }
  }
}