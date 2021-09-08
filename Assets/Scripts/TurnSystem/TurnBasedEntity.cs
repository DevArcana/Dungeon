using UnityEngine;

namespace TurnSystem
{
  public class TurnBasedEntity : MonoBehaviour
  {
    public string entityName = "Unnamed";
    public Sprite portrait;
    
    public int initiative = 0;

    private void Start()
    {
      TurnManager.Instance.RegisterTurnBasedEntity(this);
    }
  }
}