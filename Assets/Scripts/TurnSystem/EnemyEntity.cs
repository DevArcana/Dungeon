using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using World.Common;
using EntityLogic;
using EntityLogic.AI;


namespace TurnSystem
{
  public class EnemyEntity : GridLivingEntity
  {
    private Queue<GridPos> _path;
    private GridPos _target;

    private void Update()
    {
      if (TurnManager.instance.CurrentTurnTaker == this && !TurnManager.instance.Transactions.HasPendingTransactions)
      {
        // var pos = MapUtils.ToMapPos(transform.position);
        // Debug.Log($"Enemy - {pos.x}, {pos.y}");
        // if (TurnManager.instance.ActionPoints.RemainingActionPoints == ActionPointsHolder.MaxActionPoints)
        // {
        //   _target = Pathfinding.FindClosestPlayer(pos);
        //   var pathFinding = new Pathfinding();
        //   var path = pathFinding.FindPath(pos, _target);
        //   _path = new Queue<GridPos>(path ?? Array.Empty<GridPos>());
        // }
        // if (!(_path is null) && _path.Count != 0)
        // {
        //   var transaction = new MoveTransaction(this, _path.Dequeue());
        //   TurnManager.instance.EnqueueTransaction(transaction);
        // }
        //
        // if (pos.OneDimDistance(_target) == 1)
        // {
        //   var map = World.World.instance;
        //   var attackedEntity = map.GetEntity(_target) as PlayerEntity;
        //   TransactionBase transaction = new AttackTransaction(this, attackedEntity, 10);
        // if (!TurnManager.instance.CanProcessTransaction(transaction))
        //   {
        //     transaction = new PassTurnTransaction(this);
        //   }
        //   TurnManager.instance.EnqueueTransaction(transaction);
        // }

        var utilityAI = new UtilityAI();
        utilityAI.PerformNextAction(this);
      }
    }
  }
}