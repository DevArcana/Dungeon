using Equipment;
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
                EntityEquipment.isEnabled = true;
                EntityEquipment.iconsGenerated = false;
            });
        }
    }
}