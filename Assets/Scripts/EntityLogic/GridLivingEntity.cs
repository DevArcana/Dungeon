using System;
using Abilities;
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

    public AbilityBase ability1;
    public AbilityBase ability2;
    public AbilityBase ability3;
    public AbilityBase abilitySpecial;

    private void Awake()
    {
      ability1 = new DoNothingAbility(this);
      ability2 = new DoNothingAbility(this);
      ability3 = new DoNothingAbility(this);
      abilitySpecial = new DoNothingAbility(this);
    }

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