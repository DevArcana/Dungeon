using System;
using EntityLogic.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class AbilityTooltip : MonoBehaviour
  {
    public Image backgroundImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI cooldownText;
    public Image cooldownSymbol;
    public TextMeshProUGUI costText;
    public Image costSymbol;

    private AbilityBase _currentAbility;

    public void Show(AbilityBase ability)
    {
      _currentAbility = ability;
      
      titleText.text = ability.title;
      descriptionText.text = ability.description;
      cooldownText.text = $"{ability.cooldown} turn{(ability.cooldown == 1 ? string.Empty : "s")}";
      costText.text = ability.GetCostForTooltip();
      
      backgroundImage.enabled = true;
      titleText.enabled = true;
      descriptionText.enabled = true;
      cooldownText.enabled = true;
      cooldownSymbol.enabled = true;
      costText.enabled = true;
      costSymbol.enabled = true;
    }
    
    public void Hide(AbilityBase ability)
    {
      if (ability != null && ability != _currentAbility)
      {
        return;
      }

      _currentAbility = null;
      
      backgroundImage.enabled = false;
      titleText.enabled = false;
      descriptionText.enabled = false;
      cooldownText.enabled = false;
      cooldownSymbol.enabled = false;
      costText.enabled = false;
      costSymbol.enabled = false;
    }
  }
}