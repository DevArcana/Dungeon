using EntityLogic;

namespace TurnSystem.Transactions
{
  public class HealAllyTransaction : TransactionBase
  {
    private readonly GridLivingEntity _healingEntity;
    private readonly GridLivingEntity _healedEntity;
    private readonly int _healAmount;

    public HealAllyTransaction(GridLivingEntity healingEntity, GridLivingEntity healedEntity, int healAmount)
    {
      _healingEntity = healingEntity;
      _healedEntity = healedEntity;
      _healAmount = healAmount;
    }

    protected override void Process()
    {
      var entityHealth = _healedEntity.GetComponent<DamageableEntity>()?.damageable;
      entityHealth?.Heal(_healAmount);
      Finish();
    }
  }
}