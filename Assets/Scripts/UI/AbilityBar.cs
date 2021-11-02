using System;
using TMPro;
using TurnSystem;
using UnityEngine;
using UnityEngine.UI;

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

    public global::Abilities.Abilities abilities;

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

      abilities = !(turnTaker is PlayerEntity) ? null : Abilities.Abilities.FromEntity(turnTaker);
    
      if (!(abilities is null))
      {
        abilities.AbilitySelectionChanged += OnAbilitySelectionChanged;
      }
      turnManager.TurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      if (!(e.Entity is PlayerEntity player))
      {
        return;
      }
    
      if (!(abilities is null))
      {
        abilities.AbilitySelectionChanged -= OnAbilitySelectionChanged;
      }

      abilities = Abilities.Abilities.FromEntity(player);
      abilities.AbilitySelectionChanged += OnAbilitySelectionChanged;
      
      RefreshAbilities();
    }

    private void OnAbilitySelectionChanged(object sender, global::Abilities.Abilities.AbilitySelectionChangedEventArgs e)
    {
      RefreshAbilities();
    }

    private void OnActionPointsChanged(object sender, EventArgs e)
    {
      RefreshAbilities();
    }

    private void SelectAbility(int abilityNumber)
    {
      if (abilities is null)
      {
        return;
      }

      if (abilities.SelectedAbilityNumber == abilityNumber)
      {
        abilities.DeselectAbility();
        return;
      }

      abilities.SelectAbility(abilityNumber);
    }

    private void RefreshAbilities()
    {
      if (abilities is null)
      {
        return;
      }

      var abilityNumber = abilities.SelectedAbilityNumber;

      var actionPoints = TurnManager.instance.ActionPoints.RemainingActionPoints;

      abilityButtonText1.text = "1";
      abilityButtonText2.text = "2";
      abilityButtonText3.text = "3";
      abilityButtonText4.text = "S";

      var ability = abilities.Slot1;
      abilityButtonImage1.color = abilityNumber == 1 ? Color.yellow : ability is null || ability.Cost > actionPoints ? Color.gray : Color.white;
      ability = abilities.Slot2;
      abilityButtonImage2.color = abilityNumber == 2 ? Color.yellow : ability is null || ability.Cost > actionPoints ? Color.gray : Color.white;
      ability = abilities.Slot3;
      abilityButtonImage3.color = abilityNumber == 3 ? Color.yellow : ability is null || ability.Cost > actionPoints ? Color.gray : Color.white;
      ability = abilities.Special;
      abilityButtonImage4.color = abilityNumber == 4 ? Color.yellow : ability is null || ability.Cost > actionPoints ? Color.gray : Color.blue;
    }
  }
}