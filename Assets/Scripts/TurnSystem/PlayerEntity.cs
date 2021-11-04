using EntityLogic;
using TurnSystem.Transactions;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class PlayerEntity : GridLivingEntity
  {
    // unity components
    private DamageableEntity _damageable;

    protected override void Start()
    {
      base.Start();
      
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
  }
}