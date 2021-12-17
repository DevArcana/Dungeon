using System;
using System.Linq;
using TurnSystem;
using UnityEngine;
using Utils;
using World.Common;

namespace UI
{
  public class TooltipSystem : MonoBehaviour
  {
    private static TooltipSystem _instance;

    public Tooltip tooltip;
    private UnityEngine.Camera _camera;

    private GridPos _pos;

    private void Awake()
    {
      _camera = UnityEngine.Camera.main;
      if (_instance != null)
      {
        Destroy(gameObject);
        return;
      }
      
      _instance = this;
    }

    private void Start()
    {
      TurnManager.instance.Transactions.TransactionsProcessed += UpdateTooltip;
    }

    private void OnDestroy()
    {
      TurnManager.instance.Transactions.TransactionsProcessed -= UpdateTooltip;
    }

    private void Update()
    {
      if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit, float.MaxValue, layerMask: LayerMask.GetMask("Default", "Triggers")))
      {
        var pos = MapUtils.ToMapPos(hit.point);

        if (_pos != pos)
        {
          _pos = pos;
          UpdateTooltip();
        }
      }
      else
      {
        Hide();
      }
    }

    private void UpdateTooltip()
    {
      var occupant = World.World.instance.GetOccupant(_pos);
      if (occupant)
      {
        Show(occupant.entityName, occupant.GetTooltip());
      }
      else
      {
        var trigger = World.World.instance.GetTriggers(_pos).FirstOrDefault();
        if (trigger)
        {
          Show(trigger.entityName, "");
        }
        else
        {
          Hide();
        }
      }
    }

    private void Show(string header, string content)
    {
      tooltip.gameObject.SetActive(true);
      tooltip.header.text = header;
      tooltip.content.text = content;
    }

    private void Hide()
    {
      tooltip.gameObject.SetActive(false);
    }
  }
}