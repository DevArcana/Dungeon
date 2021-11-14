using EntityLogic;

namespace TurnSystem.Transactions
{
  public class FireballTransaction : TransactionBase
  {
    private readonly GridLivingEntity _attackingEntity;
    private readonly GridLivingEntity _attackedEntity;
    private readonly int _damage;

    public FireballTransaction(GridLivingEntity attackingEntity, GridLivingEntity attackedEntity, int damage)
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