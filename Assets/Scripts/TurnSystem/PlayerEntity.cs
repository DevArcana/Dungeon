using System.Linq;
using Abilities;
using Camera;
using EntityLogic;
using Transactions;
using UnityEngine;
using Utils;
using Abilities = Abilities.Abilities;

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
      TurnManager.instance.EnqueueTransaction(new ChangeSceneTransaction("DeathScene"));
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

          if (!turnManager.CanProcessTransaction(transaction))
          {
            transaction = new RangedAttackTransaction(this, 2, 15, projectilePrefab, enemy, equipment.weapon.range, impactPrefab);
          }
          
          turnManager.EnqueueTransaction(transaction);
        }
        else
        {
          var transaction = new MoveTransaction(this, mapPos);
          TurnManager.instance.EnqueueTransaction(transaction);
        }
      }
      else if (Input.GetButtonDown("Jump"))
      {
        _damageable.damageable.SufferDamage(10);
      }
    }
  }
}