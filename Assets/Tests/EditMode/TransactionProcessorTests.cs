using NUnit.Framework;
using TurnSystem;
using TurnSystem.Transactions;

namespace Tests.EditMode
{
  public class TransactionProcessorTests
  {
    private class CounterTransaction : TransactionBase
    {
      public int counter;
      public int startCounter;
      public int processCounter;
      public int endCounter;

      public CounterTransaction(int counter) : base(false)
      {
        this.counter = counter;
      }

      protected override void Start()
      {
        startCounter++;
      }

      protected override void Process()
      {
        processCounter++;
        
        counter--;

        if (counter <= 0)
        {
          Finish();
        }
      }

      protected override void End()
      {
        endCounter++;
      }
    }

    [Test]
    public void ProcessTransactionsInQueue()
    {
      var t1 = new CounterTransaction(5);
      var t2 = new CounterTransaction(7);

      var sut = new TransactionProcessor();
      
      sut.EnqueueTransaction(t1);
      sut.EnqueueTransaction(t2);
      
      while (sut.HasPendingTransactions)
      {
        sut.ProcessTransactions();
      }
      
      Assert.That(sut.HasPendingTransactions, Is.False);
      
      Assert.That(t1.startCounter, Is.EqualTo(1));
      Assert.That(t1.processCounter, Is.EqualTo(5));
      Assert.That(t1.endCounter, Is.EqualTo(1));
      
      Assert.That(t2.startCounter, Is.EqualTo(1));
      Assert.That(t2.processCounter, Is.EqualTo(7));
      Assert.That(t2.endCounter, Is.EqualTo(1));
    }
  }
}