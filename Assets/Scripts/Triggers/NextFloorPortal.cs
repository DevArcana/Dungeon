using World.Entities;

namespace Triggers
{
  public class NextFloorPortal : GridTriggerEntity
  {
    public override void OnTrigger(GridEntity gridEntity)
    {
      World.World.instance.LoadNextFloor();
    }
  }
}