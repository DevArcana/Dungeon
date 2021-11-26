using System;
using System.Collections.Generic;
using UI;

namespace Equipment
{
    [Serializable]
    public class AttributeUpgrade
    {
        public Attribute attribute;
        public int value;
    }
    public class WeaponComponent : Item
    {
        public List<AttributeUpgrade> attributesUpgrades;
        [NonSerialized] public RecipeType recipeType;
        
        public WeaponComponent()
        {
            ability = null;
        }

        public string AttributeNames()
        {
            string res = "";
            foreach (var upgrade in attributesUpgrades)
            {
                res += $"{upgrade.attribute}:\n";
            }
            return res.TrimEnd();
        }
        
        public string AttributeValues()
        {
            string res = "";
            foreach (var upgrade in attributesUpgrades)
            {
                res += $"{upgrade.value}\n";
            }
            return res.TrimEnd();
        }
    }
}