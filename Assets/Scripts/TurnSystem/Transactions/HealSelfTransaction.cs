using EntityLogic;

namespace TurnSystem.Transactions
{
  public class HealSelfTransaction : TransactionBase
  {
    private readonly GridLivingEntity _entity;
    private readonly int _healAmount;

    public HealSelfTransaction(GridLivingEntity entity, int healAmount)
    {
      _entity = entity;
      _healAmount = healAmount;
    }

    protected override void Process()
    {
      var entityHealth = _entity.GetComponent<DamageableEntity>()?.damageable;
      entityHealth?.Heal(_healAmount);
      Finish();
    }
  }
}