using Codice.Client.BaseCommands;
using Equipment;
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
                EntityEquipment.isEnabled = false;
                EntityEquipment.isItemDescriptionEnabled = false;
            });
        }
    }
}