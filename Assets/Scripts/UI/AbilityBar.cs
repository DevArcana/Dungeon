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
      foreach (var ability in _abilities)
      {
        ability.button.enabled = false;
        ability.button.interactable = false;
      }
    }

    private void OnAbilityFinishedExecution()
    {
      foreach (var ability in _abilities)
      {
        ability.button.enabled = true;
        ability.button.interactable = true;
      }
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
      var turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      if (_abilityOwner == turnTaker)
      {
        return;
      }

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