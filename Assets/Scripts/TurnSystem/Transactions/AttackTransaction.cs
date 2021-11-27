using EntityLogic;

namespace TurnSystem.Transactions
{
  public class AttackTransaction : TransactionBase
  {
    private readonly GridLivingEntity _attackingEntity;
    private readonly GridLivingEntity _attackedEntity;

    public AttackTransaction(GridLivingEntity attackingEntity, GridLivingEntity attackedEntity, bool isAbility) : base(isAbility)
    {
      _attackingEntity = attackingEntity;
      _attackedEntity = attackedEntity;
    }

    protected override void Process()
    {
      var damage = _attackingEntity.attributes.WeaponDamage;
      var victimHealth = _attackedEntity.health;
      victimHealth?.SufferDamage(damage);
      Finish();
    }
  }
}