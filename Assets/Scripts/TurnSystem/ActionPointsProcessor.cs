﻿using System;
using UnityEngine;

namespace TurnSystem
{
  public class ActionPointsProcessor
  {
    public const int MaxActionPoints = 5;
    public int ActionPoints { get; private set; }
    public int ReservedActionPoints { get; private set; }

    public event Action<int> ActionPointsChanged;
    public event Action<int> ActionPointsReserved;

    public ActionPointsProcessor()
    {
      ResetPoints();
    }

    public void ReservePoints(int points)
    {
      if (points < 0)
      {
        points = 0;
      }
      
      ReservedActionPoints = points;
      ActionPointsReserved?.Invoke(ReservedActionPoints);
    }

    public bool CanSpendReservedPoints()
    {
      return ReservedActionPoints <= ActionPoints && ReservedActionPoints != 0;
    }

    public void SpendReservedPoints()
    {
      if (!CanSpendReservedPoints())
      {
        return;
      }
      
      ActionPoints -= ReservedActionPoints;
      ReservedActionPoints = 0;

      ActionPointsChanged?.Invoke(ActionPoints);
      ActionPointsReserved?.Invoke(ReservedActionPoints);
    }

    public void ResetPoints()
    {
      ActionPoints = MaxActionPoints;
      ReservedActionPoints = 0;
      ActionPointsChanged?.Invoke(ActionPoints);
      ActionPointsReserved?.Invoke(ReservedActionPoints);
    }
  }
}