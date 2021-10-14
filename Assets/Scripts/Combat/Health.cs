using System;
using UnityEngine;

namespace Combat
{
  public class Health : MonoBehaviour
  {
    public int maxHealth = 100;
    public int health;

    public class HealthChangedEventArgs : EventArgs
    {
      public readonly int health;
      
      public HealthChangedEventArgs(int health)
      {
        this.health = health;
      }
    }
    
    /// <summary>
    /// Fired every time the health changes.
    /// </summary>
    public event EventHandler<HealthChangedEventArgs> HealthChanged;

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
      health = amount;
      HealthChanged?.Invoke(this, new HealthChangedEventArgs(health));
    }

    private void Start()
    {
      SetHealth(maxHealth);
    }
  }
}