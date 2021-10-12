using UnityEngine;

namespace World.Entities
{
  public class GridTriggerEntity : GridEntity
  {
    public virtual void OnTrigger(GridEntity gridEntity)
    {
      Debug.Log("Triggered!");
    }
  }
}