using Map.Utilities;
using UnityEngine;

namespace TurnSystem
{
  public class TurnBasedEntity : MonoBehaviour
  {
    public string entityName = "Unnamed";
    public Sprite portrait;
    public GameObject highlight;
    
    public int initiative = 0;

    protected virtual void Start()
    {
      highlight = transform.Find("Highlight").gameObject;
      Highlighted(false);
      TurnManager.Instance.RegisterTurnBasedEntity(this);
    }

    public void Highlighted(bool active)
    {
      highlight.SetActive(active);
    }

    protected void Move(Vector3 point)
    {
      var pos = MapUtils.ToWorldPos(MapUtils.ToMapPos(point));
      if (pos != null)
      {
        transform.position = pos.Value;
      }
    }
  }
}