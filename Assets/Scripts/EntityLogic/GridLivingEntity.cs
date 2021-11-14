using System.Collections.Generic;
using EntityLogic.Abilities;
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

    public EntityEquipment equipment;

    public List<AbilityBase> abilities;
    public List<int> AbilityCooldowns { get; private set; }

    protected virtual void Start()
    {
      AbilityCooldowns = new List<int>();
      foreach (var _ in abilities)
      {
        AbilityCooldowns.Add(0);
      }
      
      highlight = transform.Find("Highlight").gameObject;
      Highlighted(false);
      
      // register
      if (autoRegister)
      {
        TurnManager.instance.RegisterTurnBasedEntity(this);
      }
    }

    private void OnDestroy()
    {
      TurnManager.instance.UnregisterTurnBasedEntity(this);
    }

    public void Highlighted(bool active)
    {
      highlight.SetActive(active);
    }

    public virtual string GetTooltip()
    {
      return "this is an entity";
    }
  }
}