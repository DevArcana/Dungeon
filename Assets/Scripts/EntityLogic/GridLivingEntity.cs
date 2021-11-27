using System.Collections.Generic;
using System.Linq;
using EntityLogic.Abilities;
using EntityLogic.Attributes;
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
    public EntityBaseAttributes baseAttributes;
    private EntityAttributes attributes;

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

    public void RecalculateAttributes()
    {
      var attributeModifiers = new List<AttributeModifier>(equipment.GetAllAttributeModifiers());
      attributeModifiers.AddRange(attributes.PermanentModifiers);
      
      // strength
      var strengthModifiers = attributeModifiers.Where(x => x.attribute == Attribute.Strength).ToList();
      attributes.Strength = CalculateAttribute(baseAttributes.strength, strengthModifiers);
      
      // agility
      var agilityModifiers = attributeModifiers.Where(x => x.attribute == Attribute.Agility).ToList();
      attributes.Agility = CalculateAttribute(baseAttributes.agility, agilityModifiers);
      
      // focus
      var focusModifiers = attributeModifiers.Where(x => x.attribute == Attribute.Focus).ToList();
      attributes.Focus = CalculateAttribute(baseAttributes.focus, focusModifiers);
      
      // initiative
      var initiativeModifiers = attributeModifiers.Where(x => x.attribute == Attribute.Initiative).ToList();
      attributes.Initiative = CalculateAttribute(baseAttributes.initiative, initiativeModifiers);
      
      // damage reduction
      //   only additive modifiers
      var damageReductionModifiers = attributeModifiers.Where(x => x.attribute == Attribute.DamageReduction).ToList();
      attributes.DamageReduction = (float)(baseAttributes.damageReduction + damageReductionModifiers.Sum(x => x.value));
    }

    private static float CalculateAttribute(float baseValue, IEnumerable<AttributeModifier> modifiers)
    {
      var additiveValue = modifiers.Where(x => x.type == ModifierType.Additive).Sum(x => x.value);
      var multiplicativeValue = modifiers.Where(x => x.type == ModifierType.Multiplicative).Sum(x => x.value);
      var value = baseValue + additiveValue;
      value *= 1 + multiplicativeValue / 100;

      return (float) value;
    }
  }
}