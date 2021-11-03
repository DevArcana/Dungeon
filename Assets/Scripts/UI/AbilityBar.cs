using System;
using Abilities;
using EntityLogic;
using TMPro;
using TurnSystem;
using UnityEngine;
using UnityEngine.UI;
using World.Interaction;

namespace UI
{
  public class AbilityBar : MonoBehaviour
  {
    public Button abilityButton1;
    public Button abilityButton2;
    public Button abilityButton3;
    public Button abilityButton4;

    public Image abilityButtonImage1;
    public Image abilityButtonImage2;
    public Image abilityButtonImage3;
    public Image abilityButtonImage4;

    public TextMeshProUGUI abilityButtonText1;
    public TextMeshProUGUI abilityButtonText2;
    public TextMeshProUGUI abilityButtonText3;
    public TextMeshProUGUI abilityButtonText4;

    public AbilityProcessor abilities;
    
    private void Start()
    {
      abilityButton1.onClick.AddListener(() =>
      {
        SelectAbility(1);
      });
      abilityButton2.onClick.AddListener(() =>
      {
        SelectAbility(2);
      });
      abilityButton3.onClick.AddListener(() =>
      {
        SelectAbility(3);
      });
      abilityButton4.onClick.AddListener(() =>
      {
        SelectAbility(4);
      });

      var turnManager = TurnManager.instance;
      turnManager.ActionPoints.ActionPointsChanged += OnActionPointsChanged;

      var turnTaker = turnManager.CurrentTurnTaker;

      if (!(turnTaker is null))
      {
        abilities = turnTaker.abilities;
      }

      turnManager.TurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      if (!(e.Entity is PlayerEntity player))
      {
        return;
      }

      abilities = player.abilities;
      
      RefreshAbilities();
    }

    private void OnActionPointsChanged(object sender, EventArgs e)
    {
      RefreshAbilities();
    }

    private void SelectAbility(int abilityNumber)
    {
      if (abilities.selectedAbilityNumber == abilityNumber)
      {
        abilities.DeselectAbility();
        return;
      }

      abilities.SelectAbility(abilityNumber);
    }

    private void RefreshAbilities()
    {
      var ability = abilities.ability1;
      abilityButtonImage1 = ability.icon;
      abilityButtonText1.text = ability.title;
      ability = abilities.ability2;
      abilityButtonImage2 = ability.icon;
      abilityButtonText2.text = ability.title;
      ability = abilities.ability3;
      abilityButtonImage3 = ability.icon;
      abilityButtonText3.text = ability.title;
      ability = abilities.abilitySpecial;
      abilityButtonImage4 = ability.icon;
      abilityButtonText4.text = ability.title;
    }
  }
}