using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EntityLogic;
using EntityLogic.Attributes;
using Equipment;
using TMPro;
using TurnSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Attribute = EntityLogic.Attributes.Attribute;

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
        
        public static bool IsEditingInputField =>
            EventSystem.current.currentSelectedGameObject?.TryGetComponent(out TMP_InputField _) ?? false;

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
            if (IsEditingInputField) return;
            _equipment = TurnManager.instance.CurrentTurnTaker.equipment;
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCraftingEnabled = !isCraftingEnabled;
                EquipmentUI.isEnabled = false;
                craftingUIGenerated = false;
                currentPage = 1;
                ClearPage();
                isComponentsDescriptionEnabled = false;
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
                    weaponAttributesNamesText.text = _equipment.weapon.AttributeNames();
                    weaponAttributesValuesText.text = _equipment.weapon.AttributeValues();
                    weaponIcon.enabled = true;
                    weaponIcon.sprite = _equipment.weapon.icon;
                }
                
                if (!isWeaponDescriptionEnabled)
                {
                    var recipePage = componentFields.FirstOrDefault(x => x.recipeType == recipeType);
                    if (!(recipePage is null))
                    {
                        if (recipePage.componentFields.TrueForAll(x => !(x.selectedComponent is null)))
                        {
                            craftButton.onClick.RemoveAllListeners();
                            
                            isWeaponDescriptionEnabled = true;
                            switch (recipeType)
                            {
                                case RecipeType.Sword:
                                {
                                    craftedWeaponIcon.sprite = swordSprite;
                                    break;
                                }
                                case RecipeType.Bow:
                                {
                                    craftedWeaponIcon.sprite = bowSprite;
                                    break;
                                }
                                case RecipeType.Axe:
                                {
                                    craftedWeaponIcon.sprite = axeSprite;
                                    break;
                                }
                            }
                            AttributeModifier[] attributeList = CalculateAttributes(recipePage.componentFields.Select(x => x.selectedComponent).ToList());
                            var attributeText = "";
                            foreach (var attribute in attributeList) 
                            { 
                                if (attribute.attribute == Attribute.WeaponDamage)
                                {
                                    attributeText = attributeText + "Damage:\n";
                                }
                                else if (attribute.attribute == Attribute.WeaponRange)
                                {
                                    attributeText = attributeText +  "Range:\n";
                                }
                                else
                                {
                                    attributeText = attributeText + attribute.attribute + ":\n";
                                }
                            }
                            var attributeValues = "";
                            foreach (var attribute in attributeList)
                            {
                                attributeValues = attributeValues + attribute.value + ":\n";
                            }
                            
                            craftButton.onClick.AddListener(() => Craft(attributeList));

                            craftedWeaponAttributesNamesText.text = attributeText;
                            craftedWeaponAttributesValuesText.text = attributeValues;
                        }
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
        
        public void ClearPage()
        {
            var currentComponentPage = componentFields.FirstOrDefault(x => x.recipeType == recipeType);
            if (!(currentComponentPage is null))
            {
                foreach (var field in currentComponentPage.componentFields)
                {
                    field.Clear();
                    isWeaponDescriptionEnabled = false;
                }
            }
        }

        private AttributeModifier[] CalculateAttributes(IEnumerable<WeaponComponent> usedComponents)
        {
            var attributeList = new List<AttributeModifier>();
            var resultAttributeList = new List<AttributeModifier>();
            foreach (var component in usedComponents)
            {
                attributeList = attributeList.Concat(component.attributeModifiers).ToList();
            }

            var attributeNamesList = attributeList.Select(x => x.attribute).Distinct().ToList();
            foreach (var attributeName in attributeNamesList)
            {
                var attribute = new AttributeModifier()
                {
                    attribute = attributeName,
                    value = attributeList.Where(x => x.attribute == attributeName).Sum(x => x.value),
                    type = ModifierType.Additive
                };
                resultAttributeList.Add(attribute);
            }

            return resultAttributeList.ToArray();
        }

        public void Craft(AttributeModifier[] attributeUpgrades)
        {
            Weapon w;
            switch (recipeType)
            {
                case RecipeType.Sword:
                {
                    w = ScriptableObject.CreateInstance<Sword>();
                    break;
                }
                case RecipeType.Bow:
                {
                    w = ScriptableObject.CreateInstance<Bow>();
                    break;
                }
                case RecipeType.Axe:
                {
                    w = ScriptableObject.CreateInstance<Axe>();
                    break;
                }
                default:
                {
                    w = null;
                    break;
                }
            }
            if (w is null) return;
            
            w.itemName = craftedWeaponName.text;
            w.description = craftedWeaponDescriptionText.text;
            w.icon = craftedWeaponIcon.sprite;
            var damage = attributeUpgrades.First(x => x.attribute == Attribute.WeaponDamage);
            w.baseDamage = (float) (damage?.value ?? 1);
            var range = attributeUpgrades.FirstOrDefault(x => x.attribute == Attribute.WeaponRange);
            w.baseRange = (float) (range?.value ?? 1);
            w.attributeModifiers = attributeUpgrades.Where(x => x.attribute!= Attribute.WeaponDamage && x.attribute != Attribute.WeaponRange).ToArray();
            
            var backpack = TurnManager.instance.CurrentTurnTaker.equipment.backpack;
            var recipePage = componentFields.FirstOrDefault(x => x.recipeType == recipeType);
            if (recipePage is null) return;
            backpack.Add(w);
            foreach (var component in recipePage.componentFields)
            {
                backpack.Remove(component.selectedComponent);
                component.selectedComponent = null;
            }

            isWeaponDescriptionEnabled = false;
            isComponentsDescriptionEnabled = false;
            craftingUIGenerated = false;
        }
    }
}