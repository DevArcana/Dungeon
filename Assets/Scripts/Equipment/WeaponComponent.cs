using System;
using UI;

namespace Equipment
{
    [Serializable]
    public class WeaponComponent : Item
    {
        [NonSerialized] public RecipeType recipeType;
        
        public WeaponComponent()
        {
            ability = null;
        }
    }
}