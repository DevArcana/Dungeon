using TurnSystem;
using TurnSystem.Transactions;

namespace EntityLogic
{
  public class PlayerEntity : GridLivingEntity
  {
    protected override void OnDeath()
    {
      base.OnDeath();
      TurnManager.instance.Transactions.EnqueueTransaction(new ChangeSceneTransaction("DeathScene", false));
    }

    private void OnDestroy()
    {
      health.EntityDied -= OnDeath;
      TurnManager.instance.UnregisterTurnBasedEntity(this);
    }
    
    public override string GetTooltip()
    {
      return $"HP: {health.Health}/{health.MaximumHealth}";
    }
  }
}