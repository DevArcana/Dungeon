using System;
using EntityLogic.Abilities;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

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
        public int level;
        public ItemRarity itemRarity;
        public string itemName;
        public string description;
        public Image icon;
        [CanBeNull] public AbilityBase ability;
    }
}