using Transactions;
using TurnSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Triggers
{
  public class NextFloorPortal : MonoBehaviour
  {
    private void OnTriggerEnter(Collider other)
    {
      TurnManager.instance.Transactions.EnqueueTransaction(new ChangeSceneTransaction(SceneManager.GetActiveScene().name));
      Destroy(gameObject);
    }
  }
}