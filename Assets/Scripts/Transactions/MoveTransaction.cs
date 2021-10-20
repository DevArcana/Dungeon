using System;
using System.Linq;
using EntityLogic;
using UnityEngine;
using Utils;
using World.Common;

namespace Transactions
{
  public class MoveTransaction : TransactionBase
  {
    private readonly GridEntity _targetEntity;
    private readonly Vector3 _targetPosition;

    private Vector3 _velocity;
    
    public MoveTransaction(GridLivingEntity movedEntity, GridPos targetPosition) : base(0, movedEntity)
    {
      _targetEntity = movedEntity;
      _targetPosition = MapUtils.ToWorldPos(targetPosition);
      _velocity = Vector3.zero;
      Cost = (int) Math.Round((movedEntity.transform.position - _targetPosition).magnitude);
    }

    public override bool CanExecute()
    {
      return World.World.instance.IsWalkable(MapUtils.ToMapPos(_targetPosition));
    }

    protected override void Start()
    {
      World.World.instance.Unregister(_targetEntity);
    }

    protected override void Process()
    {
      var transform = _targetEntity.transform;
      var position = transform.position;
      var difference = _targetPosition - position;
      var distance = difference.sqrMagnitude;
      
      if (distance > 0.0001f)
      {
        transform.position = Vector3.SmoothDamp(position, _targetPosition, ref _velocity, 0.15f, 100.0f);
        transform.rotation = Quaternion.LookRotation(difference.normalized);
      }
      else
      {
        var forward = transform.forward;
        forward.y = 0;
        transform.rotation = Quaternion.LookRotation(forward.normalized);
        Finish();
      }
    }

    protected override void End()
    {
      World.World.instance.Register(_targetEntity);
    }
  }
}