using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CloseCraftingButton : MonoBehaviour
    {
        public Button craftingButton;
        public CraftingUI craftingUI;
        
        private void Start()
        {
            craftingButton.onClick.AddListener(() =>
            {
                CraftingUI.isCraftingEnabled = false;
                craftingUI.ClearPage();
                CraftingUI.isComponentsDescriptionEnabled = false;
            });
        }
    }
}