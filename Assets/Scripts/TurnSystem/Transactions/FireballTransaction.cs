using EntityLogic;

namespace TurnSystem.Transactions
{
  public class FireballTransaction : TransactionBase
  {
    private readonly GridLivingEntity _attackedEntity;
    private readonly float _damage;

    public FireballTransaction(GridLivingEntity attackedEntity, float damage, bool isAbility) : base(isAbility)
    {
      _attackedEntity = attackedEntity;
      _damage = damage;
    }

    protected override void Process()
    {
      var victimHealth = _attackedEntity.health;
      var victimDamageReduction = _attackedEntity.attributes.DamageReduction;
      victimHealth?.SufferDamage(_damage, victimDamageReduction);
      Finish();
    }
  }
}