﻿using EntityLogic;

namespace TurnSystem.Transactions
{
  public class DoNothingTransaction : TransactionBase
  {
    public DoNothingTransaction(GridLivingEntity owner, bool isAbility) : base(isAbility)
    {
    }
  }
}