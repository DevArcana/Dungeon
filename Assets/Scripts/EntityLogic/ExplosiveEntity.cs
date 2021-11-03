using Transactions;
using TurnSystem;
using UnityEngine;

namespace EntityLogic
{
  [RequireComponent(typeof(GridLivingEntity))]
  [RequireComponent(typeof(DamageableEntity))]
  public class ExplosiveEntity : MonoBehaviour
  {
    public int radius = 5;
    
    private DamageableEntity _damageableEntity;
    private GridEntity _entity;

    private void Start()
    {
      _entity = GetComponent<GridEntity>();
      _damageableEntity = GetComponent<DamageableEntity>();

      _damageableEntity.damageable.EntityDied += OnDeath;
    }

    private void OnDestroy()
    {
      _damageableEntity.damageable.EntityDied -= OnDeath;
    }

    private void OnDeath()
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new ExplosionTransaction(_entity.GridPos, radius));
    }
  }
}