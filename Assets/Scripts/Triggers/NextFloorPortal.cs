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
      if (!other.CompareTag("Player"))
      {
        return;
      }
      
      TurnManager.instance.Transactions.EnqueueTransaction(new ChangeSceneTransaction(SceneManager.GetActiveScene().name));
      Destroy(gameObject);
    }
  }
}