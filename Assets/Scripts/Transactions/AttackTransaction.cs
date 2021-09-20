using Combat;
using Grid;

namespace Transactions
{
  public class AttackTransaction : TransactionBase
  {
    private readonly GridEntity _attackingEntity;
    private readonly GridEntity _attackedEntity;
    private readonly int _damage;

    public AttackTransaction(GridEntity attackingEntity, GridEntity attackedEntity, int damage) : base(1, attackingEntity)
    {
      _attackingEntity = attackingEntity;
      _attackedEntity = attackedEntity;
      _damage = damage;
    }

    public override bool CanExecute()
    {
      var offenderPosition = _attackingEntity.transform.position;
      var victimPosition = _attackedEntity.transform.position;
      var difference = offenderPosition - victimPosition;
      var distance = difference.magnitude;

      return distance <= 1.0f;
    }

    protected override void Process()
    {
      var victimHealth = _attackedEntity.GetComponent<Health>();
      if (victimHealth != null)
      {
        victimHealth.SufferDamage(_damage);
      }
      
      Finish();
    }
  }
}