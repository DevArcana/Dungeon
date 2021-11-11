using UnityEngine;

namespace Equipment
{
    public class RangedWeapon : Weapon
    {
        public RangedWeapon()
        {
            isRanged = true;
        }
        public Sprite projectilePrefab;
    }
}