using System;
using TurnSystem;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
  public class TileHighlight : MonoBehaviour
  {
    private GameObject _tile;
    private UnityEngine.Camera _camera;

    private void Awake()
    {
      _tile = transform.Find("Tile").gameObject;
      _camera = UnityEngine.Camera.main;
      Highlighted(false);
    }

    private void Update()
    {
      if (TurnManager.Instance.CurrentTurnTaker is PlayerEntity)
      {
        Highlighted(true);
        if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
        {
          var x = math.floor(hit.point.x) + 0.5f;
          var z = math.floor(hit.point.z) + 0.5f;

          transform.position = new Vector3(x, 0, z);
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