using EntityLogic;

namespace TurnSystem.Transactions
{
    public class PassTurnTransaction : TransactionBase
    {
        public PassTurnTransaction(GridLivingEntity owner, bool isAbility) : base(isAbility)
        {
        }
    }
}