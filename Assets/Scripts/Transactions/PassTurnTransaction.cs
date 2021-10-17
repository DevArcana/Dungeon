using EntityLogic;
using TurnSystem;

namespace Transactions
{
    public class PassTurnTransaction : TransactionBase
    {
        public PassTurnTransaction(GridLivingEntity owner) : base(0, owner)
        {
            Cost = TurnManager.instance.ActionPoints.RemainingActionPoints;
        }
    }
}