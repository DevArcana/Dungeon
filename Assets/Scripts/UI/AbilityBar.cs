using System.Collections.Generic;
using EntityLogic;
using EntityLogic.Abilities;
using TurnSystem;
using UnityEngine;

namespace UI
{
  public class AbilityBar : MonoBehaviour
  {
    public Ability abilityPrefab;

    private List<Ability> _abilities;
    private GridLivingEntity _abilityOwner;
    
    private void Start()
    {
      Destroy(transform.GetChild(0).gameObject);

      var turnManager = TurnManager.instance;
      turnManager.ActionPoints.ActionPointsChanged += OnActionPointsChanged;
      turnManager.TurnChanged += OnTurnChanged;
      
      var abilityProcessor = AbilityProcessor.instance;
      abilityProcessor.AbilityStartedExecution += OnAbilityStartedExecution;
      abilityProcessor.AbilityFinishedExecution += OnAbilityFinishedExecution;

      _abilities = new List<Ability>();
      
      Refresh();
    }

    private void OnAbilityStartedExecution()
    {
      Refresh();
    }

    private void OnAbilityFinishedExecution()
    {
      Refresh();
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      Refresh();
    }

    private void OnActionPointsChanged(int points)
    {
      Refresh();
    }

    private void SelectAbility(int abilityNumber)
    {
      var abilityProcessor = AbilityProcessor.instance;
      
      if (abilityProcessor.SelectedAbilityIndex == abilityNumber)
      {
        abilityProcessor.DeselectAbility();
        return;
      }

      abilityProcessor.SelectAbility(abilityNumber);
    }

    private void Refresh()
    {
      var turnManager = TurnManager.instance;
      var turnTaker = turnManager.CurrentTurnTaker;

      if (_abilityOwner != turnTaker)
      {
        Repopulate();
      }

      if (!(turnTaker is PlayerEntity))
      {
        return;
      }
      
      var abilityProcessor = AbilityProcessor.instance;
      
      for (var i = 0; i < turnTaker.abilities.Count; i++)
      {
        var ability = turnTaker.abilities[i];
        var offCooldown = turnTaker.AbilityCooldowns[i] == 0;
        var canAfford = ability.GetMinimumPossibleCost() <= turnManager.ActionPoints.ActionPoints;
        var castable = !abilityProcessor.AbilityInExecution;
        
        _abilities[i].button.enabled = offCooldown && canAfford && castable;
        _abilities[i].button.interactable = offCooldown && canAfford && castable;
        _abilities[i].text.text = !offCooldown && castable ? turnTaker.AbilityCooldowns[i].ToString() : string.Empty;
      }
    }

    private void Repopulate()
    {
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      Clear();
      _abilityOwner = null;

      if (!(turnTaker is PlayerEntity))
      {
        return;
      }
      
      _abilityOwner = turnTaker;

      for (var i = 0; i < turnTaker.abilities.Count; i++)
      {
        var ability = turnTaker.abilities[i];
        
        var instantiatedAbility = Instantiate(abilityPrefab, transform);
        var index = i;
        instantiatedAbility.button.onClick.AddListener(() =>
        {
          SelectAbility(index);
        });
        instantiatedAbility.image.sprite = ability.icon;
        
        _abilities.Add(instantiatedAbility);
      }
    }

    private void Clear()
    {
      foreach (var ability in _abilities)
      {
        Destroy(ability.gameObject);
      }
      
      _abilities.Clear();
    }
  }
}