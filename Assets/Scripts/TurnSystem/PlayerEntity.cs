using Grid;
using Map;
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
        var occupant = WorldDataProvider.Instance.GetData(mapPos)?.occupant;

        if (occupant != null && occupant is EnemyEntity enemy)
        {
          var transaction = new AttackTransaction(this, enemy, 20);
          TurnManager.Instance.EnqueueTransaction(transaction);
        }
        else
        {
          var transaction = new MoveTransaction(this, mapPos);
          TurnManager.Instance.EnqueueTransaction(transaction);
        }
      }
    }
  }
}