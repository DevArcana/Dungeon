using System;

namespace TurnSystem
{
  public class ActionPointsProcessor
  {
    public const int MaxActionPoints = 5;
    public int ActionPoints { get; private set; }
    public int ReservedActionPoints { get; private set; }

    public event Action<int> ActionPointsChanged;
    public event Action<int> ActionPointsReserved;

    public void ReservePoints(int points)
    {
      ReservedActionPoints = points;
      ActionPointsReserved?.Invoke(ReservedActionPoints);
    }

    public bool SpendReservedPoints()
    {
      if (ReservedActionPoints > ActionPoints)
      {
        return false;
      }
      
      ActionPoints -= ReservedActionPoints;
      ReservedActionPoints = 0;

      ActionPointsChanged?.Invoke(ReservedActionPoints);
      ActionPointsReserved?.Invoke(ReservedActionPoints);
      
      return true;
    }

    public void ResetPoints()
    {
      ActionPoints = MaxActionPoints;
      ReservedActionPoints = 0;
      ActionPointsChanged?.Invoke(ReservedActionPoints);
      ActionPointsReserved?.Invoke(ReservedActionPoints);
    }
  }
}