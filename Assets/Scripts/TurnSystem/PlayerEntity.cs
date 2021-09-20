using Grid;
using Transactions;
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
        var mapPos = MapUtils.ToMapPos(hit.point);
        SelectionManager.Instance.Highlight(mapPos);
        var transaction = new MoveTransaction(this, MapUtils.ToWorldPos(mapPos));
        TurnManager.Instance.EnqueueTransaction(transaction);
      }
    }
  }
}