using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;

namespace TurnSystem
{
  public class EnemyEntity : TurnBasedEntity
  {
    private void Update()
    {
      if (TurnManager.Instance.CurrentTurnTaker == this)
      {
        Move(transform.position + new Vector3(1, 0, 0));
        TurnManager.Instance.NextTurn();
      }
    }
  }
}