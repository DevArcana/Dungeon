using Camera;
using UnityEngine;

namespace TurnSystem.Transactions
{
  public class PanCameraTransaction : TransactionBase
  {
    private readonly Vector3? _targetPosition;
    private readonly bool _unlockAfter;
    private DivineCamera _camera;

    public PanCameraTransaction(Vector3? targetPosition = null, bool unlockAfter = true)
    {
      _targetPosition = targetPosition;
      _unlockAfter = unlockAfter;
    }

    protected override void Start()
    {
      if (!UnityEngine.Camera.main)
      {
        return;
      }

      _camera = UnityEngine.Camera.main.GetComponent<DivineCamera>();
      _camera.LockMovement();
      _camera.PanToLocation(_targetPosition);
    }

    protected override void Process()
    {
      if (!_camera.IsMoving())
      {
        Finish();
      }
    }

    protected override void End()
    {
      if (_unlockAfter)
      {
        _camera.UnlockMovement();
      }
    }
  }
}