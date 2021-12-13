using System;
using EntityLogic;
using EntityLogic.AI;
using Equipment;

namespace TurnSystem.Transactions
{
    public class EquipItemTransaction : TransactionBase
    {
        private readonly GridLivingEntity _entity;
        private readonly Item _item;
        
        public EquipItemTransaction(GridLivingEntity entity, Item item, bool isAbility) : base(isAbility)
        {
            _entity = entity;
            _item = item;
        }

        protected override void Process()
        {
            base.Process();
            _entity.equipment.Equip(_item);
            LogConsole.Log($"{_item.itemName} equipped." + Environment.NewLine);
            Finish();
        }
    }
}