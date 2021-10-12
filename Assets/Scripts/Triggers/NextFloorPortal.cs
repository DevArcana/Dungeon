using Transactions;
using TurnSystem;
using World.Entities;

namespace Triggers
{
  public class NextFloorPortal : GridTriggerEntity
  {
    public string scene;
    public override void OnTrigger(GridEntity gridEntity)
    {
      TurnManager.instance.EnqueueTransaction(new ChangeSceneTransaction(scene));
    }
  }
}