using UnityEngine;
using World.Common;

namespace Transactions
{
  public class ExplosionTransaction : TransactionBase
  {
    public ExplosionTransaction(GridPos center, int radius)
    {
    }

    protected override void Process()
    {
      Debug.Log("explosion!");
      Finish();
    }
  }
}