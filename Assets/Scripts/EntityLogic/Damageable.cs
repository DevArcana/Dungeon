using System;
using UnityEngine;

namespace EntityLogic
{
  [Serializable]
  public class Damageable
  {
    [SerializeField]
    private int maxHealth;
    
    [SerializeField]
    private int health;

    public Damageable(int maxHealth = 100)
    {
      health = this.maxHealth = maxHealth;
    }
    
    /// <summary>
    /// Maximum amount of hit points an entity can have
    /// </summary>
    public int MaxHealth => maxHealth;
    
    /// <summary>
    /// Current amount of hit points an entity has
    /// </summary>
    public int Health => health;

    /// <summary>
    /// Fired every time the health changes.
    /// </summary>
    public event Action HealthChanged;

    /// <summary>
    /// Fired when entity's health reaches zero.
    /// </summary>
    public event Action EntityDied;

    /// <summary>
    /// Causes the entity to suffer damage.
    /// </summary>
    /// <param name="damage">The amount of damage dealt to the entity.</param>
    public void SufferDamage(int damage)
    {
      SetHealth(Math.Max(health - damage, 0));
    }

    /// <summary>
    /// Causes the entity to restore health. Entity's health cannot exceed their maximum health.
    /// </summary>
    /// <param name="heal">The amount of health restored by the entity.</param>
    public void Heal(int heal)
    {
      SetHealth(Math.Min(health + heal, maxHealth));
    }

    /// <summary>
    /// Sets the health of an entity and notifes the game via an event.
    /// </summary>
    /// <param name="amount">The new amount of health to give the entity.</param>
    public void SetHealth(int amount)
    {
      if (health == amount)
      {
        return;
      }
      
      health = amount;
      HealthChanged?.Invoke();

      if (health == 0)
      {
        EntityDied?.Invoke();
      }
    }
  }
}