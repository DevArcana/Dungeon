using EntityLogic;

namespace TurnSystem.Transactions
{
  public class HealSelfTransaction : TransactionBase
  {
    private readonly GridLivingEntity _entity;
    private readonly float _healAmount;

    public HealSelfTransaction(GridLivingEntity entity, float healAmount, bool isAbility) : base(isAbility)
    {
      _entity = entity;
      _healAmount = healAmount;
    }

    protected override void Process()
    {
      var entityHealth = _entity.health;
      entityHealth?.Heal(_healAmount);
      Finish();
    }
  }
}