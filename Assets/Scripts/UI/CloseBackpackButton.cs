using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CloseBackpackButton : MonoBehaviour
    {
        public Button backpackButton;
        
        private void Start()
        {
            backpackButton.onClick.AddListener(() =>
            {
                EquipmentUI.isEnabled = false;
                EquipmentUI.isItemDescriptionEnabled = false;
            });
        }
    }
}