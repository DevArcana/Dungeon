using UI;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "Pommel", menuName = "Components/Sword/Pommel", order = 4)]
    public class Pommel : WeaponComponent
    {
        public Pommel()
        {
            recipeType = RecipeType.Sword;
        }
    }
}
