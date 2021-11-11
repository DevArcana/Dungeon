using UnityEngine;

namespace Equipment
{
    public enum Attribute
    {
        Strength,
        Agility,
        Focus,
        Initiative,
        HealthPoints
    }
    [CreateAssetMenu(fileName = "UpgradePotion", menuName = "Consumable/UpgradePotion", order = 2)]
    public class UpgradePotion : Consumable
    {
        public Attribute attribute;
        public int amount;

        public UpgradePotion()
        {
            
        }
    }
}