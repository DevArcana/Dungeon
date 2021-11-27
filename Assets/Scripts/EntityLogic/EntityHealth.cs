using System;

namespace EntityLogic
{
  public class EntityHealth
  {
    public float Health { get; set; }
    public float MaximumHealth { get; set; }

    public EntityHealth(float health = 100)
    {
      Health = MaximumHealth = health;
    }

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
    public void SufferDamage(float damage)
    {
      SetHealth(Math.Max(Health - damage, 0));
    }

    /// <summary>
    /// Causes the entity to restore health. Entity's health cannot exceed their maximum health.
    /// </summary>
    /// <param name="heal">The amount of health restored by the entity.</param>
    public void Heal(float heal)
    {
      SetHealth(Math.Min(Health + heal, MaximumHealth));
    }

    /// <summary>
    /// Sets the health of an entity and notifes the game via an event.
    /// </summary>
    /// <param name="amount">The new amount of health to give the entity.</param>
    public void SetHealth(float amount)
    {
      if (Health == amount)
      {
        return;
      }
      
      Health = amount;
      HealthChanged?.Invoke();

      if (Health == 0)
      {
        EntityDied?.Invoke();
      }
    }
  }
}