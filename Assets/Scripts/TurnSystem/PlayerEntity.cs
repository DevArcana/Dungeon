using EntityLogic;
using TurnSystem.Transactions;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class PlayerEntity : GridLivingEntity
  {
    // prefabs
    public GameObject projectilePrefab;
    public GameObject impactPrefab;
    
    // unity components
    private UnityEngine.Camera _camera;
    private DamageableEntity _damageable;

    protected override void Start()
    {
      base.Start();
      
      _camera = UnityEngine.Camera.main;
      _damageable = GetComponent<DamageableEntity>();

      _damageable.damageable.EntityDied += OnDeath;
    }
    
    private void OnDestroy()
    {
      if (_damageable != null)
      {
        _damageable.damageable.EntityDied -= OnDeath;
      }
    }

    private static void OnDeath()
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new ChangeSceneTransaction("DeathScene"));
    }

    private void Update()
    {
      if (Input.GetMouseButtonDown(0) && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
      {
        var mapPos = MapUtils.ToMapPos(hit.point);
        var enemy = World.World.instance.GetEntity(mapPos) as EnemyEntity;

        if (enemy != null)
        {
          var turnManager = TurnManager.instance;
          TransactionBase transaction = new AttackTransaction(this, enemy, 20);
          turnManager.Transactions.EnqueueTransaction(transaction);
        }
        else
        {
          var transaction = new MoveTransaction(this, mapPos);
          TurnManager.instance.Transactions.EnqueueTransaction(transaction);
        }
      }
      else if (Input.GetButtonDown("Jump"))
      {
        _damageable.damageable.SufferDamage(10);
      }
    }
  }
}