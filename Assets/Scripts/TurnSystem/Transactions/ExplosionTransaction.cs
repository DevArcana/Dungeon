using UnityEngine;
using World.Common;

namespace TurnSystem.Transactions
{
  public class ExplosionTransaction : TransactionBase
  {
    public ExplosionTransaction(GridPos center, int radius, bool isAbility) : base(isAbility)
    {
    }

    protected override void Process()
    {
      Debug.Log("explosion!");
      Finish();
    }
  }
}