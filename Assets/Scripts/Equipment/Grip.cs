using UI;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "Grip", menuName = "Components/Sword/Grip", order = 3)]
    public class Grip : WeaponComponent
    {
        public Grip()
        {
            recipeType = RecipeType.Sword;
        }
    }
}