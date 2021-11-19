using NUnit.Framework;
using TurnSystem.Transactions;

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

    [Test]
    public void CallAllMethodsDuringSingleRunWhenFinishedImmediately()
    {
      var transaction = new CounterTransaction(0);
      Assert.That(transaction.Run(), Is.EqualTo(true));
      Assert.That(transaction.startCounter, Is.EqualTo(1));
      Assert.That(transaction.processCounter, Is.EqualTo(1));
      Assert.That(transaction.endCounter, Is.EqualTo(1));
    }

    [Test]
    public void ProcessIsNotCalledAfterFinished()
    {
      var transaction = new CounterTransaction(0);
      Assert.That(transaction.Run(), Is.EqualTo(true));

      for (var i = 0; i < 3; i++)
      {
        Assert.That(transaction.Run(), Is.EqualTo(true));
      }
      
      Assert.That(transaction.startCounter, Is.EqualTo(1));
      Assert.That(transaction.processCounter, Is.EqualTo(1));
      Assert.That(transaction.endCounter, Is.EqualTo(1));
    }

    [Test]
    public void EndCalledDirectlyAfterFinish()
    {
      var transaction = new CounterTransaction(2);
      Assert.That(transaction.Run(), Is.EqualTo(false));
      
      Assert.That(transaction.startCounter, Is.EqualTo(1));
      Assert.That(transaction.processCounter, Is.EqualTo(1));
      Assert.That(transaction.endCounter, Is.EqualTo(0));
      
      Assert.That(transaction.Run(), Is.EqualTo(true));
      
      Assert.That(transaction.startCounter, Is.EqualTo(1));
      Assert.That(transaction.processCounter, Is.EqualTo(2));
      Assert.That(transaction.endCounter, Is.EqualTo(1));
    }
  }
}