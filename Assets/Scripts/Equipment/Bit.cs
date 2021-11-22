using UI;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "Bit", menuName = "Components/Axe/Bit", order = 2)]
    public class Bit : WeaponComponent
    {
        public Bit()
        {
            recipeType = RecipeType.Axe;
        }
    }
}