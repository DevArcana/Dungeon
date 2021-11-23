using System.IO;
using EntityLogic.AI;
using TurnSystem;
using UnityEngine;
using World.Common;

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
    
    // unity components
    private DamageableEntity _damageable;

    protected override void Start()
    {
      base.Start();
      _damageable = GetComponent<DamageableEntity>();
      var personality = GetComponent<PersonalityProvider>();
      teamwork = personality.teamwork;
      aggressiveness = personality.aggressiveness;
    }

    public override string GetTooltip()
    {
      return $"HP: {_damageable.damageable.Health}/{_damageable.damageable.MaxHealth}";
    }
  }
}