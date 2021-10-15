using EntityLogic;
using Transactions;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class EnemyEntity : GridLivingEntity
  {
    private void Update()
    {
      if (TurnManager.instance.CurrentTurnTaker == this && !TurnManager.instance.TransactionPending)
      {
        var pos = MapUtils.ToMapPos(transform.position);
        var transaction = new MoveTransaction(this,  pos.North);
        TurnManager.instance.EnqueueTransaction(transaction);
      }
    }
  }
}