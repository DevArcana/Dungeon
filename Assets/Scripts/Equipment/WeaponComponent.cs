using System;
using System.Collections.Generic;
using UI;

namespace Equipment
{
    public class WeaponComponent : Item
    {
        public List<PlayerAttribute> playerAttributesList;
        public List<int> playerAttributeValues;
        public List<WeaponAttribute> weaponAttributesList;
        public List<int> weaponAttributeValues;
        [NonSerialized] public RecipeType recipeType;
        
        public WeaponComponent()
        {
            ability = null;
        }

        public string AttributeNames()
        {
            string res = "";
            foreach (var name in playerAttributesList)
            {
                res += $"{name}:\n";
            }
            foreach (var name in weaponAttributesList)
            {
                res += $"{name}:\n";
            }
            return res.TrimEnd();
        }
        
        public string AttributeValues()
        {
            string res = "";
            foreach (var values in playerAttributeValues)
            {
                res += $"{values}\n";
            }
            foreach (var values in weaponAttributeValues)
            {
                res += $"{values}\n";
            }
            return res.TrimEnd();
        }
    }
}