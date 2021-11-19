using System;
using System.Linq;
using TurnSystem;
using UnityEngine;
using World.Common;

namespace EntityLogic.Abilities
{
  public class AbilityProcessor : MonoBehaviour
  {
    public static AbilityProcessor instance;
    
    public AbilityBase implicitAbility;

    public AbilityBase SelectedAbility { get; private set; }
    public int SelectedAbilityIndex { get; private set; }

    public event Action<IAbility, int> SelectedAbilityChanged;
    public event Action AbilityStartedExecution;
    public event Action AbilityFinishedExecution;

    private void OnAbilityStartedExecution()
    {
      AbilityStartedExecution?.Invoke();
    }
    private void OnAbilityFinishedExecution()
    {
      AbilityFinishedExecution?.Invoke();
    }

    public bool AbilityInExecution { get; private set; }

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Destroy(gameObject);
        return;
      }
      
      var turnManager = TurnManager.instance;
      turnManager.Transactions.AbilityTransactionsProcessed += OnAbilityTransactionsProcessed;
      turnManager.TurnChanged += OnTurnChanged;
      
      DeselectAbility();
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      DeselectAbility();
      
      for (var i = 0; i < e.Entity.AbilityCooldowns.Count; i++)
      {
        var currentValue = e.Entity.AbilityCooldowns[i];
        e.Entity.AbilityCooldowns[i] = currentValue == 0 ? 0 : currentValue - 1;
      }
    }

    private void OnAbilityTransactionsProcessed()
    {
      if (AbilityInExecution)
      {
        AbilityInExecution = false;
        DeselectAbility();
        OnAbilityFinishedExecution();
      }
    }

    public AbilityBase GetAbility(int index)
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      return index >= 0 && index < turnTaker.abilities.Count ? turnTaker.abilities[index] : null;
    }

    public bool SelectAbility(int index)
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      if (index >= 0 && index < turnTaker.abilities.Count)
      {
        if (turnTaker.AbilityCooldowns[index] != 0)
        {
          return false;
        }
        
        SelectedAbilityIndex = index;
        SelectedAbility = turnTaker.abilities[index];
        SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
      }
      else
      {
        DeselectAbility();
      }
      return true;
    }

    public bool SelectAbility(Type ability)
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      for (var i = 0; i < turnTaker.abilities.Count; i++)
      {
        if (turnTaker.abilities[i].GetType() != ability)
        {
          continue;
        }

        if (turnTaker.AbilityCooldowns[i] != 0)
        {
          return false;
        }
        
        SelectedAbility = turnTaker.abilities[i];
        SelectedAbilityIndex = i;
        SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
        return true;
      }

      return false;
    }

    public void DeselectAbility()
    {
      if (SelectedAbilityIndex == -1)
      {
        return;
      }
      
      SelectedAbility = implicitAbility;
      SelectedAbilityIndex = -1;
      SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
    }

    public bool CanExecute(GridPos pos)
    {
      TurnManager.instance.ActionPoints.ReservePoints(SelectedAbility.GetEffectiveCost(pos));
      
      return !AbilityInExecution
             && SelectedAbility.GetValidTargetPositions().Contains(pos)
             && TurnManager.instance.ActionPoints.CanSpendReservedPoints()
             && SelectedAbility.CanExecute(pos);
    }

    public void Execute(GridPos pos)
    {
      if (!CanExecute(pos))
      {
        return;
      }
      
      var turnManager = TurnManager.instance;
      if (SelectedAbilityIndex != -1)
      {
        turnManager.CurrentTurnTaker.AbilityCooldowns[SelectedAbilityIndex] = SelectedAbility.cooldown;
      }
      AbilityInExecution = true;
      OnAbilityStartedExecution();
      
      TurnManager.instance.ActionPoints.SpendReservedPoints();
      var selectedAbility = SelectedAbility;
      selectedAbility.Execute(pos);
    }
  }
}