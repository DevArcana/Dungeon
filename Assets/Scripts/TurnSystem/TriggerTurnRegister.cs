using System;
using System.Linq;
using UnityEngine;
using World.Entities;

namespace TurnSystem
{
  public class TriggerTurnRegister : MonoBehaviour
  {
    public string faction = "enemies";
    public GridLivingEntity[] entities;

    private void OnTriggerEnter(Collider other)
    {
      Debug.Log(other.name);
      var gridEntity = other.GetComponent<GridLivingEntity>();
      if (gridEntity != null && gridEntity.faction != faction)
      {
        foreach (var entity in entities)
        {
          TurnManager.instance.RegisterTurnBasedEntity(entity);
        }
        
        Destroy(gameObject);
      }
    }
  }
}