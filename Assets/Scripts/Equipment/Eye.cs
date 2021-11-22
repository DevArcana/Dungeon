using UI;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "Eye", menuName = "Components/Axe/Eye", order = 3)]
    public class Eye : WeaponComponent
    {
        public Eye()
        {
            recipeType = RecipeType.Axe;
        }
    }
}