using EntityLogic;

namespace TurnSystem.Transactions
{
  public class HealAllyTransaction : TransactionBase
  {
    private readonly GridLivingEntity _healedEntity;
    private readonly float _healAmount;

    public HealAllyTransaction(GridLivingEntity healedEntity, float healAmount, bool isAbility) : base(isAbility)
    {
      _healedEntity = healedEntity;
      _healAmount = healAmount;
    }

    protected override void Process()
    {
      var entityHealth = _healedEntity.health;
      entityHealth?.Heal(_healAmount);
      Finish();
    }
  }
}