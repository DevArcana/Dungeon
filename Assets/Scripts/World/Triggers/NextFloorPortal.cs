using EntityLogic;
using TurnSystem;
using TurnSystem.Transactions;
using UnityEngine.SceneManagement;

namespace World.Triggers
{
  public class NextFloorPortal : GridTriggerEntity
  {
    
    private void Start()
    {
      entityName = "Next Floor Portal";
    }
    
    public override void OnTileEntered(GridLivingEntity entity)
    {
      if (entity == null || !(entity is PlayerEntity))
      {
        return;
      }
      
      TurnManager.instance.Transactions.EnqueueTransaction(new ChangeSceneTransaction(SceneManager.GetActiveScene().name, false));
      Destroy(gameObject);
    }

    public override string GetTooltip()
    {
      return "Next level staircase";
    }
  }
}