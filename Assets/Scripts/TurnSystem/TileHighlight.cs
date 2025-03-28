﻿using EntityLogic;
using UnityEngine;
using Utils;

namespace TurnSystem
{
  public class TileHighlight : MonoBehaviour
  {
    private GameObject _tile;
    private UnityEngine.Camera _camera;

    private void Awake()
    {
      _tile = transform.GetChild(0).gameObject;
      _camera = UnityEngine.Camera.main;
      Highlighted(false);
    }

    private void Update()
    {
      if (TurnManager.instance.CurrentTurnTaker is PlayerEntity)
      {
        Highlighted(true);
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
        {
          var x = Mathf.Round(hit.point.x);
          var y = hit.point.y;
          var z = Mathf.Round(hit.point.z);

          var map = World.World.instance;
          if (map != null)
          {
            y = map.GetHeightAt(MapUtils.ToMapPos(hit.point));
          }

          transform.position = new Vector3(x, y, z);
        }
        else
        {
          Highlighted(false);
        }
      }
      else
      {
        Highlighted(false);
      }
    }

    private void Highlighted(bool active)
    {
      _tile.SetActive(active);
    }
  }
}