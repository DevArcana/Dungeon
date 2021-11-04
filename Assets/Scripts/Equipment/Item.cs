using System;
using EntityLogic.Abilities;
using JetBrains.Annotations;
using UnityEngine.UIElements;

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
    [Serializable]
    public class Item
    {
        public int level;
        public ItemRarity itemRarity;
        public string name;
        public string description;
        public Image icon;
        [CanBeNull] public AbilityBase ability;
    }
}