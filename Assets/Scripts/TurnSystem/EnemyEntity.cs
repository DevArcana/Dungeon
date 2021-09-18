using TurnSystem.Transactions;
using UnityEngine;

namespace TurnSystem
{
  public class EnemyEntity : TurnBasedEntity
  {
    private void Update()
    {
      if (TurnManager.Instance.CurrentTurnTaker == this && !TurnManager.Instance.TransactionPending)
      {
        var transaction = new MoveTransaction(this, transform.position + new Vector3(1, 0, 0));
        TurnManager.Instance.EnqueueTransaction(transaction);
      }
    }
  }
}