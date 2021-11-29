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

    public EntityHealth health;

    public EntityEquipment equipment;
    public EntityBaseAttributes baseAttributes;
    public EntityAttributes attributes;

    public List<AbilityBase> abilities;
    public List<int> AbilityCooldowns { get; private set; }

    private void Awake()
    {
      health = new EntityHealth(baseAttributes.maximumHealth);
      attributes = new EntityAttributes();
      RecalculateAttributes();
      
      AbilityCooldowns = new List<int>();
      foreach (var _ in abilities)
      {
        AbilityCooldowns.Add(0);
      }
    }

    protected virtual void Start()
    {
      health.EntityDied += OnDeath;
      
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
      health.EntityDied -= OnDeath;
    }

    protected virtual void OnDeath()
    {
      Destroy(gameObject);
      health.EntityDied -= OnDeath;
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
      
      // weapon damage
      var weaponDamageModifiers = attributeModifiers.Where(x => x.attribute == Attribute.WeaponDamage).ToList();
      attributes.WeaponDamage = equipment.weapon is null ? 10 : CalculateAttribute(equipment.weapon.baseDamage, weaponDamageModifiers);
      
      // weapon range
      //  only additive modifiers
      var weaponRangeModifiers = attributeModifiers.Where(x => x.attribute == Attribute.WeaponRange).ToList();
      attributes.WeaponRange = equipment.weapon is null ? 1 : (float)(equipment.weapon.baseRange + weaponRangeModifiers.Sum(x => x.value));
      
      // maximum health
      var maximumHealthModifiers = attributeModifiers.Where(x => x.attribute == Attribute.MaximumHealth).ToList();
      health.MaximumHealth = CalculateAttribute(baseAttributes.maximumHealth, maximumHealthModifiers);
    }

    private static float CalculateAttribute(float baseValue, List<AttributeModifier> modifiers)
    {
      var additiveValue = modifiers.Where(x => x.type == ModifierType.Additive).Sum(x => x.value);
      var multiplicativeValue = modifiers.Where(x => x.type == ModifierType.Multiplicative).Sum(x => x.value);
      var value = baseValue + additiveValue;
      value *= 1 + multiplicativeValue / 100;

      return (float) value;
    }
  }
}