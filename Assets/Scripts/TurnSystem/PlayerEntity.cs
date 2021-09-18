using Grid;
using TurnSystem.Transactions;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class PlayerEntity : GridEntity
  {
    private UnityEngine.Camera _camera;

    protected override void Start()
    {
      base.Start();
      
      _camera = UnityEngine.Camera.main;
    }
    
    private void Update()
    {
      if (Input.GetMouseButtonDown(0) && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
      {
        var transaction = new MoveTransaction(this, MapUtils.ToWorldPos(MapUtils.ToMapPos(hit.point)));
        TurnManager.Instance.EnqueueTransaction(transaction);
      }
    }
  }
}