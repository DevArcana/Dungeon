using EntityLogic;
using Equipment;

namespace TurnSystem.Transactions
{
    public class UnEquipItemTransaction : TransactionBase
    {
        private readonly GridLivingEntity _entity;
        private readonly Item _item;
        
        public UnEquipItemTransaction(GridLivingEntity entity, Item item, bool isAbility) : base(isAbility)
        {
            _entity = entity;
            _item = item;
        }

        protected override void Process()
        {
            base.Process();
            _entity.equipment.UnEquip(_item);
            Finish();
        }
    }
}