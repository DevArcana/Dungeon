using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace TurnSystem
{
  public class PlayerEntity : TurnBasedEntity
  {
    private UnityEngine.Camera _camera;

    protected override void Start()
    {
      base.Start();
      
      _camera = UnityEngine.Camera.main;
    }
    
    private void Update()
    {
      if (TurnManager.Instance.CurrentTurnTaker == this)
      {
        if (Input.GetMouseButtonDown(0))
        {
          if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
          {
            Move(hit.point);
            
            TurnManager.Instance.NextTurn();
          }
        }
      }
    }
  }
}