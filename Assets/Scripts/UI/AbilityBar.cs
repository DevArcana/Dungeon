using EntityLogic.Abilities;
using TurnSystem;
using UnityEngine;

namespace UI
{
  public class AbilityBar : MonoBehaviour
  {
    public Ability abilityPrefab;

    private Ability[] _abilities;

    private AbilityProcessor _currentAbilityProcessor;
    
    private void Start()
    {
      Destroy(transform.GetChild(0).gameObject);

      var turnManager = TurnManager.instance;
      turnManager.ActionPoints.ActionPointsChanged += OnActionPointsChanged;
      turnManager.TurnChanged += OnTurnChanged;

      _abilities = new Ability[6];
      for (var i = 0; i < 6; i++)
      {
        _abilities[i] = Instantiate(abilityPrefab, transform);
        var index = i;
        _abilities[i].button.onClick.AddListener(() =>
        {
          SelectAbility(index);
        });
      }
      
      var turnTaker = turnManager.CurrentTurnTaker;

      if (turnTaker is PlayerEntity)
      {
        _currentAbilityProcessor = turnTaker.abilities;
        RefreshAbilities();
      }
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      ClearAbilities();
      
      if (!(e.Entity is PlayerEntity player))
      {
        return;
      }

      _currentAbilityProcessor = player.abilities;
      RefreshAbilities();
    }

    private void OnActionPointsChanged(int points)
    {
      RefreshAbilities();
    }

    private void SelectAbility(int abilityNumber)
    {
      if (_currentAbilityProcessor.SelectedAbilityIndex == abilityNumber)
      {
        _currentAbilityProcessor.DeselectAbility();
        return;
      }

      _currentAbilityProcessor.SelectAbility(abilityNumber);
    }

    private void RefreshAbilities()
    {
      if (_currentAbilityProcessor?.abilities is null)
      {
        return;
      }
      
      for (var i = 0; i < _currentAbilityProcessor.abilities.Count; i++)
      {
        var ability = _currentAbilityProcessor.abilities[i];
      
        _abilities[i].image.sprite = ability.icon;
        _abilities[i].image.enabled = true;
      }
    }

    private void ClearAbilities()
    {
      foreach (var ability in _abilities)
      {
        ability.image.enabled = false;
      }
    }
  }
}