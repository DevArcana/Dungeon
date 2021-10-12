using UnityEngine.SceneManagement;

namespace Transactions
{
  public class ChangeSceneTransaction : TransactionBase
  {
    private readonly string _scene;
    
    public ChangeSceneTransaction(string scene) : base(0)
    {
      _scene = scene;
    }

    protected override void Process()
    {
      SceneManager.LoadScene(_scene, LoadSceneMode.Single);
      Finish();
    }
  }
}