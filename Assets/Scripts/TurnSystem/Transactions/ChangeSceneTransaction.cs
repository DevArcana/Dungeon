using UI;
using UnityEngine;

namespace TurnSystem.Transactions
{
  public class ChangeSceneTransaction : TransactionBase
  {
    private readonly string _scene;
    
    public ChangeSceneTransaction(string scene)
    {
      _scene = scene;
    }

    protected override void Process()
    {
      var obj = (GameObject) Object.Instantiate(Resources.Load("Level Loader"));
      obj.GetComponent<SceneLoader>().scene = _scene;
      Finish();
    }
  }
}