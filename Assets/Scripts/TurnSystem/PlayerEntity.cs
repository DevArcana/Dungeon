using Grid;
using Map;
using Transactions;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class PlayerEntity : GridEntity
  {
    public GameObject projectilePrefab;
    public GameObject impactPrefab;
    
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
          var turnManager = TurnManager.Instance;
          TransactionBase transaction = new AttackTransaction(this, enemy, 20);

          if (!turnManager.CanProcessTransaction(transaction))
          {
            transaction = new RangedAttackTransaction(this, 2, 15, projectilePrefab, enemy, range, impactPrefab);
          }
          
          turnManager.EnqueueTransaction(transaction);
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