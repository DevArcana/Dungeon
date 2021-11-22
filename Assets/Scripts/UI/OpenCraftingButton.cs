using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OpenCraftingButton : MonoBehaviour
    {
        public Button craftingButton;
        void Start()
        {
            craftingButton.onClick.AddListener(() =>
            {
                CraftingUI.isCraftingEnabled = true;
                CraftingUI.craftingUIGenerated = false;
                CraftingUI.currentPage = 1;
            });
        }
    }
}