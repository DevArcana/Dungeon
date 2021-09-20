using Grid;
using Transactions;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class EnemyEntity : GridEntity
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