﻿using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TabGroup : MonoBehaviour
    {
        public CraftingUI craftingUI;
        public List<TabsButton> tabsButtons;
        public Color tabIdle;
        public Color tabHover;
        public Color tabActive;
        public TabsButton selectedTab;
        public List<GameObject> tabs;

        public void Subscribe(TabsButton button)
        {
            if (tabsButtons == null)
            {
                tabsButtons = new List<TabsButton>();
                selectedTab = button;
            }
            
            tabsButtons.Add(button);
        }

        public void OnTabEnter(TabsButton button)
        {
            ResetTabs();
            if (selectedTab == null || button != selectedTab)
            {
                button.background.color = tabHover;
            }
        }

        public void OnTabExit(TabsButton button)
        {
            ResetTabs();
            
        }

        public void OnTabSelected(TabsButton button)
        {
            if (!(selectedTab is null))
            {
                craftingUI.ClearPage();
            }
            selectedTab = button;
            ResetTabs();
            button.background.color = tabActive;
            var index = button.transform.GetSiblingIndex();
            for (var i = 0; i < tabs.Count; i++)
            {
                tabs[i].SetActive(i == index);
            }

            CraftingUI.currentPage = 1;
            CraftingUI.craftingUIGenerated = false;
            CraftingUI.recipeType = button.recipeType;
            CraftingUI.isComponentsDescriptionEnabled = false;
        }

        private void ResetTabs()
        {
            foreach (var button in tabsButtons)
            {
                if (selectedTab != null && button == selectedTab)
                {
                    continue;
                }
                button.background.color = tabIdle;
            }
        }


        
    }
}
