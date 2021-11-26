using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "UpgradePotion", menuName = "Consumable/UpgradePotion", order = 2)]
    public class UpgradePotion : Consumable
    {
        public Attribute attribute;
        public int amount;

        public UpgradePotion()
        {
            
        }

        public override void Use()
        {
            base.Use();
            //TODO
        }
    }
}