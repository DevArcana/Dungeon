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
    
    public IAbility moveAbility = new ImplicitAbility();

    public IAbility SelectedAbility { get; private set; }
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

    private bool abilityInExecution;

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
      
      DeselectAbility();
      TurnManager.instance.Transactions.TransactionsProcessed += OnTransactionsProcessed;
    }

    private void OnTransactionsProcessed()
    {
      if (abilityInExecution)
      {
        abilityInExecution = false;
        OnAbilityFinishedExecution();
      }
    }

    public AbilityBase GetAbility(int index)
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      return index >= 0 && index < turnTaker.abilities.Count ? turnTaker.abilities[index] : null;
    }

    public void SelectAbility(int index)
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      if (index >= 0 && index < turnTaker.abilities.Count)
      {
        SelectedAbilityIndex = index;
        SelectedAbility = turnTaker.abilities[index];
        SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
      }
      else
      {
        DeselectAbility();
      }
    }

    public bool SelectAbility(Type ability)
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      for (var i = 0; i < turnTaker.abilities.Count; i++)
      {
        if (turnTaker.abilities[i].GetType() != ability) continue;
        SelectedAbilityIndex = i;
        SelectedAbility = turnTaker.abilities[i];
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
      
      SelectedAbility = moveAbility;
      SelectedAbilityIndex = -1;
      SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
    }

    public bool CanExecute(GridPos pos)
    {
      TurnManager.instance.ActionPoints.ReservePoints(SelectedAbility.GetEffectiveCost(pos));
      
      return !abilityInExecution
             && SelectedAbility.GetValidTargetPositions().Contains(pos)
             && TurnManager.instance.ActionPoints.CanSpendReservedPoints()
             && SelectedAbility.CanExecute(pos);
    }

    public void Execute(GridPos pos)
    {
      abilityInExecution = true;
      OnAbilityStartedExecution();
      
      TurnManager.instance.ActionPoints.SpendReservedPoints();
      var selectedAbility = SelectedAbility;
      DeselectAbility();
      selectedAbility.Execute(pos);
    }
  }
}