using Equipment;
using Map;
using Map.Legacy;
using TurnSystem;
using UnityEngine;
using Utils;

namespace Grid
{
  public abstract class GridEntity : MonoBehaviour
  {
    public string entityName = "Unnamed";
    public Sprite portrait;
    public GameObject highlight;
    
    public int initiative = 0;

    public Weapon weapon;

    public GridPos GridPos => MapUtils.ToMapPos(transform.position);

    protected virtual void Start()
    {
      highlight = transform.Find("Highlight").gameObject;
      Highlighted(false);
      
      // register
      TurnManager.Instance.RegisterTurnBasedEntity(this);
      var data = WorldDataProvider.Instance.GetData(GridPos);
      if (data != null)
      {
        data.occupant = this;
      }
    }

    public void Highlighted(bool active)
    {
      highlight.SetActive(active);
    }
  }
}