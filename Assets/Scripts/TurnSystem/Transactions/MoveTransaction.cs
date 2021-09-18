﻿using Grid;
using Map;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace TurnSystem.Transactions
{
  public class MoveTransaction : TransactionBase
  {
    private readonly GridEntity _targetEntity;
    private readonly Vector3 _targetPosition;

    private Vector3 _velocity;
    
    public MoveTransaction(GridEntity movedEntity, Vector3 targetPosition) : base((int) math.round((movedEntity.transform.position - targetPosition).magnitude), movedEntity)
    {
      _targetEntity = movedEntity;
      _targetPosition = targetPosition;
      _velocity = Vector3.zero;
    }

    public override bool CanExecute()
    {
      return WorldDataProvider.Instance.GetData(MapUtils.ToMapPos(_targetPosition))?.occupant == null;
    }

    protected override void Start()
    {
      var tile = WorldDataProvider.Instance.GetData(_targetEntity.GridPos);
      if (tile != null)
      {
        tile.occupant = null;
      }
    }

    protected override void Process()
    {
      var transform = _targetEntity.transform;
      var position = transform.position;
      var difference = _targetPosition - position;
      var distance = difference.sqrMagnitude;
      
      if (distance > 0.01f)
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
      var tile = WorldDataProvider.Instance.GetData(MapUtils.ToMapPos(_targetPosition));
      if (tile != null)
      {
        tile.occupant = _targetEntity;
      }
    }
  }
}