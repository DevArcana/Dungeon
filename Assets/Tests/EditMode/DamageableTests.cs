using EntityLogic;
using NUnit.Framework;

namespace Tests.EditMode
{
  public class DamageableTests
  {
    [Test]
    public void DefaultHealthIs100()
    {
      var sut = new Damageable();
      Assert.That(sut.Health, Is.EqualTo(100));
    }
    
    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void HealthIsInitializedToMaxHealthByDefault(int health)
    {
      var sut = new Damageable(health);
      Assert.That(sut.Health, Is.EqualTo(sut.MaxHealth));
    }

    [Test]
    public void EventIsEmittedOnEachHealthChange()
    {
      var sut = new Damageable();

      var count = 0;
      void OnHealthChange()
      {
        count++;
      }

      sut.HealthChanged += OnHealthChange;
      sut.SufferDamage(10);
      sut.Heal(10);
      sut.SetHealth(50);
      sut.HealthChanged -= OnHealthChange;
      
      Assert.That(count, Is.EqualTo(3));
    }
    
    [TestCase(1)]
    [TestCase(100)]
    [TestCase(235)]
    public void EventIsNotEmittedWhenHealthDoesNotChange(int health)
    {
      var sut = new Damageable(health);

      var count = 0;
      void OnHealthChange()
      {
        count++;
      }

      sut.HealthChanged += OnHealthChange;
      sut.SetHealth(health);
      sut.HealthChanged -= OnHealthChange;
      
      Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void DeathEventIsEmittedWhenHealthReachesZero()
    {
      var sut = new Damageable();

      var died = false;
      void OnDeath()
      {
        died = true;
      }

      sut.EntityDied += OnDeath;
      sut.SufferDamage(100);
      sut.EntityDied -= OnDeath;
      
      Assert.That(died, Is.True);
    }
    
    [Test]
    public void DeathEventIsEmittedWhenHealthReachesBelowZero()
    {
      var sut = new Damageable();

      var died = false;
      void OnDeath()
      {
        died = true;
      }

      sut.EntityDied += OnDeath;
      sut.SufferDamage(1000);
      sut.EntityDied -= OnDeath;
      
      Assert.That(died, Is.True);
    }
    
    [Test]
    public void DeathEventIsEmittedAfterHealthChangeEvent()
    {
      var sut = new Damageable();

      var e = string.Empty;
      void OnDeath()
      {
        e = "death";
      }
      
      void OnHealthChange()
      {
        e = "change";
      }
      
      sut.EntityDied += OnDeath;
      sut.HealthChanged += OnHealthChange;
      sut.SufferDamage(1000);
      sut.EntityDied -= OnDeath;
      sut.HealthChanged -= OnHealthChange;
      
      Assert.That(e, Is.EqualTo("death"));
    }

    [Test]
    public void TakingDamageDecreasesHealth()
    {
      var sut = new Damageable(50);
      sut.SufferDamage(30);
      Assert.That(sut.Health, Is.EqualTo(20));
    }
    
    [Test]
    public void HealingIncreasesHealth()
    {
      var sut = new Damageable(50);
      sut.SufferDamage(49);
      sut.Heal(9);
      Assert.That(sut.Health, Is.EqualTo(10));
    }
    
    [Test]
    public void CanNotOverHeal()
    {
      var sut = new Damageable(50);
      sut.Heal(1000);
      Assert.That(sut.Health, Is.EqualTo(50));
    }
  }
}