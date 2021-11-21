using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CloseCraftingButton : MonoBehaviour
    {
        public Button craftingButton;
        
        private void Start()
        {
            craftingButton.onClick.AddListener(() =>
            {
                CraftingUI.isCraftingEnabled = false;
            });
        }
    }
}