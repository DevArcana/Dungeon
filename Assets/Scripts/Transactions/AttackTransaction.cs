using UnityEngine;
using EntityLogic;

namespace Transactions
{
  public class AttackTransaction : TransactionBase
  {
    private readonly GridEntity _attackingEntity;
    private readonly GridEntity _attackedEntity;
    private readonly int _damage;

    public AttackTransaction(GridLivingEntity attackingEntity, GridLivingEntity attackedEntity, int damage) : base(1, attackingEntity)
    {
      _attackingEntity = attackingEntity;
      _attackedEntity = attackedEntity;
      _damage = damage;
    }

    public override bool CanExecute()
    {
      var offenderPosition = _attackingEntity.transform.position;
      var victimPosition = _attackedEntity.transform.position;
      var difference = offenderPosition - victimPosition;
      var distance = difference.magnitude;
      var heightDifference = Mathf.Abs(offenderPosition.y - victimPosition.y);
      const float epsilon = 0.1f;

      return distance <= 1.5f + epsilon && heightDifference < epsilon;
    }

    protected override void Process()
    {
      var victimHealth = _attackedEntity.GetComponent<Damageable>();
      if (victimHealth != null)
      {
        victimHealth.SufferDamage(_damage);
      }
      
      Finish();
    }
  }
}