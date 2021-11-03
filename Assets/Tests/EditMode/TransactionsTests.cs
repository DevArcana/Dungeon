using NUnit.Framework;
using Transactions;

namespace Tests.EditMode
{
  public class TransactionsTests
  {
    private class CounterTransaction : TransactionBase
    {
      public int counter;
      public int startCounter;
      public int processCounter;
      public int endCounter;

      public CounterTransaction(int counter)
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
    public void ShouldCallStartExactlyOnce()
    {
      var transaction = new CounterTransaction(10);

      while (!transaction.Run())
      {
        
      }
      
      Assert.That(transaction.startCounter, Is.EqualTo(1));
    }
    
    [Test]
    public void ShouldCallEndExactlyOnce()
    {
      var transaction = new CounterTransaction(10);

      while (!transaction.Run())
      {
        
      }
      
      Assert.That(transaction.endCounter, Is.EqualTo(1));
    }
    
    [TestCase(0, 1)]
    [TestCase(1, 1)]
    [TestCase(10, 10)]
    public void ShouldCallProcessNTimes(int c, int e)
    {
      var transaction = new CounterTransaction(c);

      while (!transaction.Run())
      {
        
      }
      
      Assert.That(transaction.processCounter, Is.EqualTo(e));
    }
  }
}