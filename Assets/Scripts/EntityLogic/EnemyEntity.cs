using System.Collections.Generic;
using EntityLogic.AI;
using TurnSystem;

namespace EntityLogic
{
  public class EnemyEntity : GridLivingEntity
  {
    public float teamwork;
    public float aggressiveness;
    public List<ActionType> currentTurnActions;
    
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
      currentTurnActions = new List<ActionType>();
      TurnManager.instance.TurnChanged += TurnChanged;
    }
    
    private void OnDestroy()
    {
      TurnManager.instance.TurnChanged -= TurnChanged;
    }

    private void TurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      if (e.PreviousEntity == this)
      {
        currentTurnActions.Clear();
      }
    }

    public override string GetTooltip()
    {
      return $"HP: {health.Health}/{health.MaximumHealth}";
    }
  }
}