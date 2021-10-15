using UnityEngine;

namespace EntityLogic
{
  public class GridTriggerEntity : GridEntity
  {
    public virtual void OnTrigger(GridEntity gridEntity)
    {
      Debug.Log("Triggered!");
    }
  }
}