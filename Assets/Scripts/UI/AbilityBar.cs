using System;
using EntityLogic;
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

    public GridLivingEntity turnTaker;
    
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

      turnTaker = turnManager.CurrentTurnTaker;

      turnManager.TurnChanged += OnTurnChanged;
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      if (!(e.Entity is PlayerEntity player))
      {
        return;
      }

      turnTaker = TurnManager.instance.CurrentTurnTaker;
      
      RefreshAbilities();
    }

    private void OnActionPointsChanged(object sender, EventArgs e)
    {
      RefreshAbilities();
    }

    private void SelectAbility(int abilityNumber)
    {
      if (turnTaker.selectedAbilityNumber == abilityNumber)
      {
        turnTaker.selectedAbility = turnTaker.abilities[0];
        return;
      }

      turnTaker.selectedAbility = turnTaker.abilities[abilityNumber];
    }

    private void RefreshAbilities()
    {
      var abilityNumber = turnTaker.selectedAbilityNumber;

      abilityButtonText1.text = "1";
      abilityButtonText2.text = "2";
      abilityButtonText3.text = "3";
      abilityButtonText4.text = "S";

      var ability = turnTaker.abilities[1];
      abilityButtonImage1.color = abilityNumber == 1 ? Color.yellow : ability is null ? Color.gray : Color.white;
      ability = turnTaker.abilities[2];
      abilityButtonImage2.color = abilityNumber == 2 ? Color.yellow : ability is null ? Color.gray : Color.white;
      ability = turnTaker.abilities[3];
      abilityButtonImage3.color = abilityNumber == 3 ? Color.yellow : ability is null ? Color.gray : Color.white;
      ability = turnTaker.abilities[4];
      abilityButtonImage4.color = abilityNumber == 4 ? Color.yellow : ability is null ? Color.gray : Color.blue;
    }
  }
}