﻿using System.Collections.Generic;
using System.Linq;
using EntityLogic.AI;
using Equipment;
using TurnSystem;
using UnityEngine;
using World.Triggers;

namespace EntityLogic
{
  public class EnemyEntity : GridLivingEntity
  {
    public float teamwork;
    public float aggressiveness;
    public List<ActionType> currentTurnActions;
    public GameObject lootBoxPrefab;
    public LootTable lootTable;

    private void Update()
    {
      var turnManager = TurnManager.instance;
      if (turnManager.CurrentTurnTaker == this
          && !turnManager.Transactions.HasPendingTransactions
          && turnManager.ActionPoints.ActionPoints > 0
          && turnManager.PeekQueue().Any(x => x is PlayerEntity))
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

    protected override void OnDeath()
    {
      base.OnDeath();
      var lootBox = Instantiate(lootBoxPrefab, transform.position, Quaternion.identity);
      lootBox.GetComponent<LootBox>().items = lootTable.GetDrop();
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
      return $"HP: {Mathf.Ceil(health.Health)}/{health.MaximumHealth}";
    }
  }
}