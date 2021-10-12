using System;
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

      _actionPoints = new ActionPoint[ActionPointsHolder.MaxActionPoints];
      for (var i = 0; i < ActionPointsHolder.MaxActionPoints; i++)
      {
        _actionPoints[i] = Instantiate(actionPointPrefab, transform);
      }
      
      Refresh();
    }

    private void OnDestroy()
    {
      TurnManager.instance.ActionPoints.ActionPointsChanged -= OnActionPointsChanged;
    }

    private void OnActionPointsChanged(object sender, EventArgs args)
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

        if (i <= ap.RemainingActionPoints)
        {
          status = ActionPointStatus.Available;
        }
        else if (i <= ap.ActionPoints)
        {
          status = ActionPointStatus.Reserved;
        }
        
        actionPoint.SetStatus(status);
      }
    }
  }
}