using EntityLogic;

namespace Transactions
{
  public class DoNothingTransaction : TransactionBase
  {
    public DoNothingTransaction(GridLivingEntity owner) : base(0, owner)
    {
    }
  }
}