using UnityEngine;
using UnityEngine.PlayerLoop;

namespace TurnSystem
{
  public class EnemyEntity : TurnBasedEntity
  {
    private void Update()
    {
      if (TurnManager.Instance.CurrentTurnTaker == this)
      {
        if (Input.GetButtonDown("Jump"))
        {
          TurnManager.Instance.NextTurn();
        }
      }
    }
  }
}