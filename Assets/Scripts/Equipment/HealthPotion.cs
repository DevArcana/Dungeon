using TurnSystem;
using TurnSystem.Transactions;
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
            TurnManager.instance.Transactions.EnqueueTransaction(
                new HealSelfTransaction(TurnManager.instance.CurrentTurnTaker, amountToHeal, false));
        }

        public override string AttributeNames()
        {
            return "Amount to Heal:";
        }

        public override string AttributeValues()
        {
            return amountToHeal.ToString();
        }
    }
}