using Transactions;
using UnityEngine;
using Utils;
using World.Entities;

namespace TurnSystem
{
  public class EnemyEntity : GridLivingEntity
  {
    private void Update()
    {
      if (TurnManager.Instance.CurrentTurnTaker == this && !TurnManager.Instance.TransactionPending)
      {
        var pos = MapUtils.ToMapPos(transform.position);
        var transaction = new MoveTransaction(this,  pos.North);
        TurnManager.Instance.EnqueueTransaction(transaction);
      }
    }
  }
}