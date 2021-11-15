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
    public AbilityTooltip tooltip;

    private List<Ability> _abilities;
    private GridLivingEntity _abilityOwner;
    
    private void Start()
    {
      Destroy(transform.GetChild(0).gameObject);

      tooltip.Hide(null);
      
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
      var selectedAbilityIndex = abilityProcessor.SelectedAbilityIndex;
      if (selectedAbilityIndex != -1)
      {
        _abilities[selectedAbilityIndex].Available();
      }
      
      if (abilityProcessor.SelectedAbilityIndex == abilityNumber)
      {
        abilityProcessor.DeselectAbility();
        return;
      }

      _abilities[abilityNumber].Selected();
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
        var abilityIcon = _abilities[i];

        if (abilityProcessor.AbilityInExecution)
        {
          abilityIcon.Processing();
        }
        else if (turnTaker.AbilityCooldowns[i] != 0)
        {
          abilityIcon.OnCooldown(turnTaker.AbilityCooldowns[i]);
        }
        else if (ability.GetMinimumPossibleCost() > turnManager.ActionPoints.ActionPoints)
        {
          abilityIcon.NotEnoughResource();
        }
        else
        {
          abilityIcon.Available();
        }
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
        instantiatedAbility.Available();
        instantiatedAbility.tooltip = tooltip;
        instantiatedAbility.ability = ability;
        
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