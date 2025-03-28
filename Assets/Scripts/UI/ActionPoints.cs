﻿using System;
using TurnSystem;
using UnityEngine;

namespace UI
{
  public class ActionPoints : MonoBehaviour
  {
    public ActionPoint actionPointPrefab;

    private ActionPoint[] _actionPoints;

    private void Start()
    {
      Destroy(transform.GetChild(0).gameObject);
      TurnManager.instance.ActionPoints.ActionPointsChanged += OnActionPointsChanged;
      TurnManager.instance.ActionPoints.ActionPointsReserved += OnActionPointsChanged;

      _actionPoints = new ActionPoint[ActionPointsProcessor.MaxActionPoints];
      for (var i = 0; i < ActionPointsProcessor.MaxActionPoints; i++)
      {
        _actionPoints[i] = Instantiate(actionPointPrefab, transform);
      }
      
      Refresh();
    }

    private void OnDestroy()
    {
      TurnManager.instance.ActionPoints.ActionPointsChanged -= OnActionPointsChanged;
      TurnManager.instance.ActionPoints.ActionPointsReserved -= OnActionPointsChanged;
    }

    private void OnActionPointsChanged(int i)
    {
      Refresh();
    }

    private void Refresh()
    {
      var ap = TurnManager.instance.ActionPoints;
      for (var i = 1; i <= _actionPoints.Length; i++)
      {
        var actionPoint = _actionPoints[i - 1];
        var status = ActionPointStatus.Spent;

        if (i <= ap.ReservedActionPoints)
        {
          status = ActionPointStatus.Reserved;
        }
        else if (i <= ap.ActionPoints)
        {
          status = ActionPointStatus.Available;
        }
        
        actionPoint.SetStatus(status);
      }
    }
  }
}