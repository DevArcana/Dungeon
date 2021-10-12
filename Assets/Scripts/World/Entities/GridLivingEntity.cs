using Equipment;
using TurnSystem;
using UnityEngine;

namespace World.Entities
{
  public class GridLivingEntity : GridEntity
  {
    public string entityName = "Unnamed";
    public Sprite portrait;
    public GameObject highlight;
    
    public int initiative = 0;

    public Weapon weapon;

    protected virtual void Start()
    {
      highlight = transform.Find("Highlight").gameObject;
      Highlighted(false);
      
      // register
      TurnManager.Instance.RegisterTurnBasedEntity(this);
      World.instance.Register(this);
    }

    public void Highlighted(bool active)
    {
      highlight.SetActive(active);
    }
  }
}