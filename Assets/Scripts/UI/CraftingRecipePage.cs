using System;
using System.Collections.Generic;
using System.Linq;
using Equipment;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Serializable]
    public class ComponentsInRecipe
    {
        public TextMeshProUGUI attributeNames;
        public TextMeshProUGUI attributeValues;
        public Image icon;
        public string componentTypeString;
        [NonSerialized] public WeaponComponent selectedComponent;

        public void Clear()
        {
            attributeNames.enabled = false;
            attributeValues.enabled = false;
            icon.enabled = false;
            selectedComponent = null;
        }
    }
    public class CraftingRecipePage : MonoBehaviour
    {
        public RecipeType recipeType;
        public List<ComponentsInRecipe> componentFields;
        public CraftingUI craftingUI;


        private void Start()
        {
            craftingUI.Subscribe(this);
            foreach (var component in componentFields)
            {
                component.icon.enabled = false;
                component.attributeNames.enabled = false;
                component.attributeValues.enabled = false;
                component.selectedComponent = null;
            }
        }

        public void Show(WeaponComponent component)
        {
            var componentField = componentFields.First(x => Type.GetType("Equipment." + x.componentTypeString) == component.GetType());
            componentField.icon.sprite = component.icon;
            componentField.icon.color = EquipmentUI.AssignRarityColor(component.itemRarity);
            componentField.icon.enabled = true;
            componentField.attributeNames.text = component.AttributeNames();
            componentField.attributeNames.enabled = true;
            componentField.attributeValues.text = component.AttributeValues();
            componentField.attributeValues.enabled = true;
            componentField.selectedComponent = component;
            craftingUI.isWeaponDescriptionEnabled = false;
            CraftingUI.craftingUIGenerated = false;
        }
    }
}