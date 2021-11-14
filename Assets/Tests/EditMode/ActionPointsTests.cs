using NUnit.Framework;
using TurnSystem;

namespace Tests.EditMode
{
  public class ActionPointsTests
  {
    [Test]
    public void ResetToMaxPoints()
    {
      var sut = new ActionPointsProcessor();

      sut.ResetPoints();
      
      Assert.That(sut.ActionPoints, Is.EqualTo(ActionPointsProcessor.MaxActionPoints));
    }
    
    [Test]
    public void ResetReservedPointsToZero()
    {
      var sut = new ActionPointsProcessor();

      sut.ResetPoints();
      
      Assert.That(sut.ReservedActionPoints, Is.Zero);
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(ActionPointsProcessor.MaxActionPoints)]
    [TestCase(int.MaxValue)]
    public void ReservePointsWithoutLimit(int reserved)
    {
      var sut = new ActionPointsProcessor();

      sut.ReservePoints(reserved);
      
      Assert.That(sut.ReservedActionPoints, Is.EqualTo(reserved));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(ActionPointsProcessor.MaxActionPoints)]
    public void SpendReservedPoints(int reserved)
    {
      var sut = new ActionPointsProcessor();

      sut.ReservePoints(reserved);

      Assert.That(sut.CanSpendReservedPoints(), Is.True);
    }

    [TestCase(ActionPointsProcessor.MaxActionPoints + 1)]
    [TestCase(int.MaxValue)]
    public void DoNotSpendMorePointsThanAvailable(int reserved)
    {
      var sut = new ActionPointsProcessor();

      sut.ReservePoints(reserved);

      Assert.That(sut.CanSpendReservedPoints(), Is.False);
    }
    
    [Test]
    public void FailToSpendZeroPoints()
    {
      var sut = new ActionPointsProcessor();
      sut.ReservePoints(0);
      Assert.That(sut.CanSpendReservedPoints(), Is.False);
    }

    [Test]
    public void InvokeEventAfterReservingPoints()
    {
      var sut = new ActionPointsProcessor();
      var reserved = 0;
      sut.ActionPointsReserved += x =>
      {
        reserved = x;
      };
      sut.ReservePoints(7);
      Assert.That(reserved, Is.EqualTo(7));
    }
    
    [Test]
    public void InvokeEventsAfterSpendingPoints()
    {
      var sut = new ActionPointsProcessor();
      var reserved = 0;
      var remaining = 0;
      
      sut.ActionPointsReserved += x =>
      {
        reserved = x;
      };
      
      sut.ActionPointsChanged += x =>
      {
        remaining = x;
      };
      
      sut.ReservePoints(ActionPointsProcessor.MaxActionPoints - 1);
      sut.SpendReservedPoints();
      
      Assert.That(reserved, Is.EqualTo(0));
      Assert.That(remaining, Is.EqualTo(1));
    }
    
    [Test]
    public void InvokeEventsAfterReset()
    {
      var sut = new ActionPointsProcessor();
      var reserved = int.MinValue;
      var remaining = int.MinValue;
      
      sut.ActionPointsReserved += x =>
      {
        reserved = x;
      };
      
      sut.ActionPointsChanged += x =>
      {
        remaining = x;
      };
      
      sut.ResetPoints();
      
      Assert.That(remaining, Is.EqualTo(ActionPointsProcessor.MaxActionPoints));
      Assert.That(reserved, Is.EqualTo(0));
    }
  }
}