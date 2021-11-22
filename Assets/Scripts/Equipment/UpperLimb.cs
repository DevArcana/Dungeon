using UI;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "UpperLimb", menuName = "Components/Bow/UpperLimb", order = 3)]
    public class UpperLimb : WeaponComponent
    {
        public UpperLimb()
        {
            recipeType = RecipeType.Bow;
        }
    }
}