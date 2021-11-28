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
        
        public virtual string AttributeNames()
        {
            var res = "";
            foreach (var mod in attributeModifiers)
            {
                if (mod.attribute == Attribute.WeaponDamage)
                {
                    res = res + "Damage:\n";
                }
                else if (mod.attribute == Attribute.WeaponRange)
                {
                    res = res +  "Range:\n";
                }
                else
                {
                    res = res + mod.attribute + ":\n";
                }
            }
            return res;
        }

        public virtual string AttributeValues()
        {
            var res = "";
            foreach (var mod in attributeModifiers)
            {
                res += mod.value;
                if (mod.type == ModifierType.Multiplicative)
                {
                    res += "%";
                }

                res += "\n";
            }
            return res;
        }
    }
}