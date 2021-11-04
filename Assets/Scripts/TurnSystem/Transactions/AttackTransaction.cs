using EntityLogic;

namespace TurnSystem.Transactions
{
  public class AttackTransaction : TransactionBase
  {
    private readonly GridEntity _attackingEntity;
    private readonly GridEntity _attackedEntity;
    private readonly int _damage;

    public AttackTransaction(GridLivingEntity attackingEntity, GridLivingEntity attackedEntity, int damage)
    {
      _attackingEntity = attackingEntity;
      _attackedEntity = attackedEntity;
      _damage = damage;
    }

    protected override void Process()
    {
      var victimHealth = _attackedEntity.GetComponent<DamageableEntity>()?.damageable;
      victimHealth?.SufferDamage(_damage);
      Finish();
    }
  }
}