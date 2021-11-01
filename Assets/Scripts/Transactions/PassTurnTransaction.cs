using EntityLogic;
using TurnSystem;

namespace Transactions
{
    public class PassTurnTransaction : TransactionBase
    {
        public PassTurnTransaction(GridLivingEntity owner) : base(TurnManager.instance.ActionPoints.RemainingActionPoints, owner)
        {
        }
    }
}