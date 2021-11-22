using TurnSystem;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "HealthPotion", menuName = "Consumable/HealthPotion", order = 1)]
    public class HealthPotion : Consumable
    {
        public int amountToHeal;
        
        public HealthPotion()
        {
            
        }

        public override void Use()
        {
            base.Use();
            //TODO
        }
    }
}