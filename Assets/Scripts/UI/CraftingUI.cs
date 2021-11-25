using System;
using System.Collections.Generic;
using System.Linq;
using Equipment;
using TMPro;
using TurnSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum RecipeType
    {
        None,
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
        public static RecipeType recipeType;
        public Sprite background;
        
        private int _numberOfSlots;
        public GameObject components;
        private GameObject[] _slots;
        public static int currentPage;
        public Button nextButton;
        public Button previousButton;
        
        public GameObject componentsDescription;
        public TextMeshProUGUI componentsName;
        public TextMeshProUGUI componentsDescriptionText;
        public TextMeshProUGUI componentsAttributesNamesText;
        public TextMeshProUGUI componentsAttributesValuesText;
        public Image componentIcon;
        public Button useButton;
        public static bool isComponentsDescriptionEnabled;
        
        public TextMeshProUGUI weaponName;
        public TextMeshProUGUI weaponDescriptionText;
        public TextMeshProUGUI weaponAttributesNamesText;
        public TextMeshProUGUI weaponAttributesValuesText;
        public Image weaponIcon;

        public List<CraftingRecipePage> componentFields;

        public GameObject craftedWeapon;
        public TextMeshProUGUI craftedWeaponName;
        public TextMeshProUGUI craftedWeaponDescriptionText;
        public TextMeshProUGUI craftedWeaponAttributesNamesText;
        public TextMeshProUGUI craftedWeaponAttributesValuesText;
        public Image craftedWeaponIcon;
        public bool isWeaponDescriptionEnabled;
        public Sprite swordSprite;
        public Sprite axeSprite;
        public Sprite bowSprite;
        public Button craftButton;

        private void Start()
        {
            isCraftingEnabled = false;
            isComponentsDescriptionEnabled = false;
            craftingUIGenerated = true;
            recipeType = RecipeType.None;
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
                    x is WeaponComponent component && component.recipeType == recipeType).ToList();
                var startingIndex = currentPage * _numberOfSlots - _numberOfSlots;
                var endingIndex = Math.Min(startingIndex + _numberOfSlots, listOfComponents.Count);
                for (var i = 0; i <= (endingIndex - 1) % _numberOfSlots; i++)
                {
                    _slots[i].GetComponent<Image>().sprite = listOfComponents[startingIndex + i].icon;
                    _slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    var x = i;
                    _slots[i].GetComponent<Button>().onClick.AddListener(() => OnItemClicked((WeaponComponent)listOfComponents[startingIndex + x]));
                }
                for (var i = ((endingIndex - 1) % _numberOfSlots) + 1; i < _numberOfSlots; i++)
                {
                    _slots[i].GetComponent<Image>().sprite = background;
                    _slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }
                craftingUIGenerated = true;
                previousButton.interactable = startingIndex != 0;
                nextButton.interactable = endingIndex != listOfComponents.Count;

                if (_equipment.weapon is null)
                {
                    weaponName.text = "No Equipped Weapon";
                    weaponDescriptionText.text = "";
                    weaponAttributesNamesText.text = "";
                    weaponAttributesValuesText.text = "";
                    weaponIcon.enabled = false;
                }
                else
                {
                    weaponName.text = _equipment.weapon.itemName;
                    weaponDescriptionText.text = _equipment.weapon.description;
                    weaponAttributesNamesText.text = "Damage:\nRange:";
                    weaponAttributesValuesText.text = $"{_equipment.weapon.damage}\n{_equipment.weapon.range}";
                    weaponIcon.enabled = true;
                    weaponIcon.sprite = _equipment.weapon.icon;
                }
                
                if (!isWeaponDescriptionEnabled)
                {
                    var recipePage = componentFields.First(x => x.recipeType == recipeType);
                    if (recipePage.componentFields.TrueForAll(x => !(x.selectedComponent is null)))
                    {
                        isWeaponDescriptionEnabled = true;
                        craftedWeaponName.text = "Some Weapon";
                        craftedWeaponDescriptionText.text = "Description";
                        craftedWeaponIcon.sprite = swordSprite;
                        //TODO attributes values
                    }
                }
            }

            
            craftedWeapon.SetActive(isWeaponDescriptionEnabled);
            MakeVisible(isCraftingEnabled);
            componentsDescription.SetActive(isComponentsDescriptionEnabled);
        }
        
         private void OnItemClicked(WeaponComponent component)
        {
            isComponentsDescriptionEnabled = true;
            componentsName.text = component.itemName;
            componentsDescriptionText.text = component.description;
            componentIcon.sprite = component.icon;
            
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() => componentFields.First(x => x.recipeType == recipeType).Show(component));

            componentsAttributesNamesText.text = component.AttributeNames();
            componentsAttributesValuesText.text = component.AttributeValues();
        }
        
        private void MakeVisible(bool isEnabled)
        {
            crafting.transform.localScale = isEnabled ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        }
        
        public void Subscribe(CraftingRecipePage craftingRecipePage)
        {
            if (componentFields == null)
            {
                componentFields = new List<CraftingRecipePage>();
            }
            
            componentFields.Add(craftingRecipePage);
        }

    }
}