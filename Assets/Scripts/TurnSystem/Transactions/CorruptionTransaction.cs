using System.Collections.Generic;
using EntityLogic;

namespace TurnSystem.Transactions
{
  public class CorruptionTransaction : TransactionBase
  {
    private readonly IEnumerable<GridLivingEntity> _entities;
    private readonly float _damage;

    public CorruptionTransaction(IEnumerable<GridLivingEntity> entities, float damage, bool isAbility) : base(isAbility)
    {
      _entities = entities;
      _damage = damage;
    }

    protected override void Process()
    {
      foreach (var entity in _entities)
      {
        var victimHealth = entity.health;
        victimHealth?.SufferDamage(_damage);
      }
      Finish();
    }
  }
}