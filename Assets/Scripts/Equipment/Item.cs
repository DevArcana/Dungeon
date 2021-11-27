using EntityLogic.Abilities;
using EntityLogic.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace Equipment
{
    public enum ItemRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Mythic
    }
    
    public class Item : ScriptableObject
    {
        public ItemRarity itemRarity;
        public string itemName;
        public string description;
        public Sprite icon;
        [CanBeNull] public AbilityBase ability;
        public AttributeModifier[] attributeModifiers;
    }
}