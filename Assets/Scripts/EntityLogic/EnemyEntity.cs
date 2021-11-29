using System.Collections.Generic;
using System.Linq;
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
      if (TurnManager.instance.CurrentTurnTaker == this && !TurnManager.instance.Transactions.HasPendingTransactions
      && TurnManager.instance.PeekQueue().Any(x => x is PlayerEntity))
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
      health.EntityDied -= OnDeath;
      TurnManager.instance.TurnChanged -= TurnChanged;
      TurnManager.instance.UnregisterTurnBasedEntity(this);
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