using System.Linq;
using Combat;
using Transactions;
using UnityEngine;
using Utils;
using World.Entities;

namespace TurnSystem
{
  [RequireComponent(typeof(Health))]
  public class PlayerEntity : GridLivingEntity
  {
    public GameObject projectilePrefab;
    public GameObject impactPrefab;
    
    private UnityEngine.Camera _camera;
    private Health _health;

    protected override void Start()
    {
      _health = GetComponent<Health>();
      base.Start();
      
      _camera = UnityEngine.Camera.main;

      _health.HealthChanged += OnHealthChanged;
    }

    private static void OnHealthChanged(object sender, Health.HealthChangedEventArgs e)
    {
      if (e.health <= 0)
      {
        TurnManager.instance.EnqueueTransaction(new ChangeSceneTransaction("DeathScene"));
      }
    }

    private void Update()
    {
      if (Input.GetMouseButtonDown(0) && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
      {
        var mapPos = MapUtils.ToMapPos(hit.point);
        var enemy = (EnemyEntity) World.World.instance.GetEntities(mapPos).FirstOrDefault(x => x is EnemyEntity);

        if (enemy != null)
        {
          var turnManager = TurnManager.instance;
          TransactionBase transaction = new AttackTransaction(this, enemy, 20);

          if (!turnManager.CanProcessTransaction(transaction))
          {
            transaction = new RangedAttackTransaction(this, 2, 15, projectilePrefab, enemy, weapon.range, impactPrefab);
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
        _health.SufferDamage(10);
      }
    }

    private void OnDestroy()
    {
      _health.HealthChanged -= OnHealthChanged;
    }
  }
}