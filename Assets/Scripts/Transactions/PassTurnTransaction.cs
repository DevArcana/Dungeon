using TurnSystem;
using World.Entities;

namespace Transactions
{
    public class PassTurnTransaction : TransactionBase
    {
        public PassTurnTransaction(GridLivingEntity owner) : base(0, owner)
        {
            Cost = TurnManager.instance.ActionPoints.ActionPoints;
        }
    }
}