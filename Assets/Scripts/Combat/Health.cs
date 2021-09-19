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