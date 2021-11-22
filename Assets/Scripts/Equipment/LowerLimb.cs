using UI;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "LowerLimb", menuName = "Components/Bow/LowerLimb", order = 2)]
    public class LowerLimb : WeaponComponent
    {
        public LowerLimb()
        {
            recipeType = RecipeType.Bow;
        }
    }
}