using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OpenBackpackButton : MonoBehaviour
    {
        public Button backpackButton;
        
        private void Start()
        {
            backpackButton.onClick.AddListener(() =>
            {
                EquipmentUI.isEnabled = true;
                EquipmentUI.iconsGenerated = false;
            });
        }
    }
}