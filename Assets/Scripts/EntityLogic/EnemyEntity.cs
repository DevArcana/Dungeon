using EntityLogic.AI;
using TurnSystem;

namespace EntityLogic
{
  public class EnemyEntity : GridLivingEntity
  {
    public float teamwork;
    public float aggressiveness;
    
    private void Update()
    {
      if (TurnManager.instance.CurrentTurnTaker == this && !TurnManager.instance.Transactions.HasPendingTransactions)
      {
        var utilityAI = new UtilityAI();
        utilityAI.PerformNextAction(this);
      }
    }

    protected override void Start()
    {
      base.Start();
      var personality = GetComponent<PersonalityProvider>();
      teamwork = personality.teamwork;
      aggressiveness = personality.aggressiveness;
    }

    public override string GetTooltip()
    {
      return $"HP: {health.Health}/{health.MaximumHealth}";
    }
  }
}