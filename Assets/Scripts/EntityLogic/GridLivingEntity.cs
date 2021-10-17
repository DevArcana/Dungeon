using System;
using Equipment;
using TurnSystem;
using UnityEngine;

namespace EntityLogic
{
  public class GridLivingEntity : GridEntity
  {
    public string entityName = "Unnamed";
    public string faction = "Enemies";
    public Sprite portrait;
    public GameObject highlight;
    public bool autoRegister = false;
    
    public int initiative = 0;

    public Weapon weapon;

    protected virtual void Start()
    {
      highlight = transform.Find("Highlight").gameObject;
      Highlighted(false);
      
      // register
      if (autoRegister)
      {
        TurnManager.instance.RegisterTurnBasedEntity(this);
      }
      
      World.World.instance.Register(this);
    }

    private void OnDestroy()
    {
      TurnManager.instance.UnregisterTurnBasedEntity(this);
    }

    public void Highlighted(bool active)
    {
      highlight.SetActive(active);
    }
  }
}