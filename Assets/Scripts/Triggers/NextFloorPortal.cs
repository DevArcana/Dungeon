using EntityLogic;
using Transactions;
using TurnSystem;
using UnityEngine.SceneManagement;

namespace Triggers
{
  public class NextFloorPortal : GridTriggerEntity
  {
    public override void OnTrigger(GridEntity gridEntity)
    {
      TurnManager.instance.EnqueueTransaction(new ChangeSceneTransaction(SceneManager.GetActiveScene().name));
    }
  }
}