using System;
using EntityLogic;
using EntityLogic.AI;
using Equipment;

namespace TurnSystem.Transactions
{
    public class RemoveItemTransaction : TransactionBase
    {
        private readonly GridLivingEntity _entity;
        private readonly Item _item;
        private readonly bool _isEquiped;
        
        public RemoveItemTransaction(GridLivingEntity entity, Item item, bool isEquiped, bool isAbility) : base(isAbility)
        {
            _entity = entity;
            _item = item;
            _isEquiped = isEquiped;
        }

        protected override void Process()
        {
            base.Process();
            _entity.equipment.RemoveItem(_item, _isEquiped);
            LogConsole.Log($"{_item.itemName} removed." + Environment.NewLine);
            Finish();
        }
    }
}