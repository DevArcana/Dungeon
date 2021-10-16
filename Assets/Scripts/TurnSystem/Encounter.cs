using EntityLogic;
using UnityEngine;

namespace TurnSystem
{
  public class Encounter : MonoBehaviour
  {
    public string faction = "enemies";
    public GridLivingEntity[] entities;

    private void OnTriggerEnter(Collider other)
    {
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