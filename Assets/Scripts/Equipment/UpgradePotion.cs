using System.Linq;
using EntityLogic.Attributes;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine;

namespace Equipment
{
    [CreateAssetMenu(fileName = "UpgradePotion", menuName = "Consumable/UpgradePotion", order = 2)]
    public class UpgradePotion : Consumable
    {
        public UpgradePotion()
        {
            
        }

        public override void Use()
        {
            base.Use();
            TurnManager.instance.Transactions.EnqueueTransaction(
                new UpgradeAttributeTransaction(TurnManager.instance.CurrentTurnTaker, attributeModifiers.First().attribute, attributeModifiers.First().value, false));
        }
    }
}