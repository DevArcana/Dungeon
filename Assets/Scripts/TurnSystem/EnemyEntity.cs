using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Transactions;
using UnityEngine;
using Utils;
using World.Common;
using EntityLogic;


namespace TurnSystem
{
  public class EnemyEntity : GridLivingEntity
  {
    private Queue<GridPos> _path;
    private GridPos _target;

    private void Update()
    {
      if (TurnManager.instance.CurrentTurnTaker == this && !TurnManager.instance.TransactionPending)
      {
        var pos = MapUtils.ToMapPos(transform.position);
        Debug.Log($"Enemy - {pos.x}, {pos.y}");
        if (TurnManager.instance.ActionPoints.ActionPoints == ActionPointsHolder.MaxActionPoints)
        {
          _target = Pathfinding.FindClosestPlayer(pos);
          var pathFinding = new Pathfinding(pos.OneDimDistance(_target), pos);
          _path = new Queue<GridPos>(pathFinding.FindPath(_target));
        }
        if (!(_path is null) && _path.Count != 0)
        {
          var transaction = new MoveTransaction(this, _path.Dequeue());
          TurnManager.instance.EnqueueTransaction(transaction);
        }

        if (pos.OneDimDistance(_target) == 1)
        {
          var map = World.World.instance;
          var attackedEntity = (PlayerEntity) map.GetEntities(_target).FirstOrDefault(x => x is PlayerEntity);
          TransactionBase transaction = new AttackTransaction(this, attackedEntity, 10);
          if (!TurnManager.instance.CanProcessTransaction(transaction))
          {
            transaction = new PassTurnTransaction(this);
          }
          TurnManager.instance.EnqueueTransaction(transaction);
        }
      }
    }
  }
}