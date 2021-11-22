using System;
using System.Collections.Generic;
using System.Linq;
using Equipment;
using TurnSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum RecipeType
    {
        Sword,
        Axe,
        Bow
    }
    public class CraftingUI : MonoBehaviour
    {
        public GameObject crafting;
        private EntityEquipment _equipment;
        public static bool isCraftingEnabled;
        public static bool craftingUIGenerated;
        private RecipeType _recipeType;
        public Sprite background;
        
        private int _numberOfSlots;
        public GameObject components;
        private GameObject[] _slots;
        public static int currentPage;
        public Button nextButton;
        public Button previousButton;
        

        private void Start()
        {
            isCraftingEnabled = false;
            _recipeType = RecipeType.Sword;
            _numberOfSlots = 6;
            currentPage = 1;
            _slots = new GameObject[_numberOfSlots];
            for (var i = 0; i < _numberOfSlots; i++)
            {
                _slots[i] = components.transform.GetChild(i).gameObject;
            }
            previousButton.onClick.AddListener(() =>
            {
                currentPage -= 1;
                craftingUIGenerated = false;
                Debug.Log(currentPage);
            });
            nextButton.onClick.AddListener(() =>
            {
                currentPage += 1;
                craftingUIGenerated = false;
                Debug.Log(currentPage);
            });
        }

        private void Update()
        {
            if (!(TurnManager.instance.CurrentTurnTaker is PlayerEntity)) return;
            _equipment = TurnManager.instance.CurrentTurnTaker.equipment;
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCraftingEnabled = !isCraftingEnabled;
                EquipmentUI.isEnabled = false;
                craftingUIGenerated = false;
                currentPage = 1;
            }

            if (isCraftingEnabled && !craftingUIGenerated)
            {
                var listOfComponents = _equipment.backpack.Where(x =>
                    x is WeaponComponent component && component.recipeType == _recipeType).ToList();
                var startingIndex = currentPage * _numberOfSlots - _numberOfSlots;
                var endingIndex = Math.Min(startingIndex + _numberOfSlots, listOfComponents.Count - startingIndex);
                for (var i = 0; i < endingIndex % (_numberOfSlots + 1); i++)
                {
                    _slots[i].GetComponent<Image>().sprite = listOfComponents[startingIndex + i].icon;
                }
                for (var i = endingIndex % (_numberOfSlots + 1); i < _numberOfSlots; i++)
                {
                    _slots[i].GetComponent<Image>().sprite = background;
                }
                craftingUIGenerated = true;
                previousButton.interactable = startingIndex != 0;
                nextButton.interactable = endingIndex != listOfComponents.Count;
            }
            MakeVisible(isCraftingEnabled);
        }
        
        private void MakeVisible(bool enabled)
        {
            if (enabled)
            {
                crafting.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                crafting.transform.localScale = new Vector3(0, 0, 0);
            }
        }
    }
}