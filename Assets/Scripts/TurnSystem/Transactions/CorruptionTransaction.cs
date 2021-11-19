using System.Collections.Generic;
using EntityLogic;

namespace TurnSystem.Transactions
{
  public class CorruptionTransaction : TransactionBase
  {
    private readonly GridLivingEntity _caster;
    private readonly IEnumerable<GridLivingEntity> _entities;
    private readonly int _damage;

    public CorruptionTransaction(GridLivingEntity caster, IEnumerable<GridLivingEntity> entities, int damage, bool isAbility) : base(isAbility)
    {
      _caster = caster;
      _entities = entities;
      _damage = damage;
    }

    protected override void Process()
    {
      foreach (var entity in _entities)
      {
        var victimHealth = entity.GetComponent<DamageableEntity>()?.damageable;
        victimHealth?.SufferDamage(_damage);
      }
      Finish();
    }
  }
}