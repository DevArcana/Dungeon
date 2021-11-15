using EntityLogic.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
  public class Ability : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
  {
    public Image image;
    public Button button;
    
    public TextMeshProUGUI cooldownText;
    public Image notEnoughResourceIcon;
    public Image selectionHighlight;

    public AbilityTooltip tooltip;
    public AbilityBase ability;
    
    public void Available()
    {
      button.enabled = true;
      button.interactable = true;
      cooldownText.text = string.Empty;
      notEnoughResourceIcon.enabled = false;
      selectionHighlight.enabled = false;
    }

    public void Processing()
    {
      button.enabled = false;
      button.interactable = false;
      cooldownText.text = string.Empty;
      notEnoughResourceIcon.enabled = false;
      selectionHighlight.enabled = false;
    }

    public void Selected()
    {
      button.enabled = true;
      button.interactable = true;
      cooldownText.text = string.Empty;
      notEnoughResourceIcon.enabled = false;
      selectionHighlight.enabled = true;
    }

    public void OnCooldown(int cooldown)
    {
      button.enabled = false;
      button.interactable = false;
      cooldownText.text = cooldown.ToString();
      notEnoughResourceIcon.enabled = false;
      selectionHighlight.enabled = false;
    }

    public void NotEnoughResource()
    {
      button.enabled = false;
      button.interactable = false;
      cooldownText.text = string.Empty;
      notEnoughResourceIcon.enabled = true;
      selectionHighlight.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      tooltip.Show(ability);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      tooltip.Hide(ability);
    }
  }
}