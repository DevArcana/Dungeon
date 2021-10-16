using EntityLogic;
using Transactions;
using TurnSystem;

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