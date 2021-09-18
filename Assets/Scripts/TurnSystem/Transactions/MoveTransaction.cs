using Unity.Mathematics;
using UnityEngine;

namespace TurnSystem.Transactions
{
  public class MoveTransaction : TransactionBase
  {
    private readonly TurnBasedEntity _targetEntity;
    private readonly Vector3 _targetPosition;

    private Vector3 _velocity;
    
    public MoveTransaction(TurnBasedEntity movedEntity, Vector3 targetPosition) : base((int) math.round((movedEntity.transform.position - targetPosition).magnitude), movedEntity)
    {
      _targetEntity = movedEntity;
      _targetPosition = targetPosition;
      _velocity = Vector3.zero;
    }

    protected override void Process()
    {
      var position = _targetEntity.transform.position;
      var distance = (position - _targetPosition).sqrMagnitude;

      if (distance > 0.01f)
      {
        _targetEntity.transform.position = Vector3.SmoothDamp(position, _targetPosition, ref _velocity, 0.15f, 100.0f);
      }
      else
      {
        Finish();
      }
    }
  }
}