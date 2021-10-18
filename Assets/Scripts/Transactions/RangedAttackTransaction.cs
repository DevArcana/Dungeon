using EntityLogic;
using UnityEngine;

namespace Transactions
{
  public class RangedAttackTransaction : TransactionBase
  {
    private readonly int _damage;
    private readonly GameObject _projectilePrefab;
    private readonly GameObject _impactPrefab;
    private readonly GridEntity _victim;
    private readonly int _range;

    private GameObject _projectile;
    private Vector3 _velocity;
    
    public RangedAttackTransaction(GridLivingEntity attacker, int cost, int damage, GameObject projectilePrefab, GridLivingEntity victim, int range, GameObject impactPrefab) : base(cost, attacker)
    {
      _damage = damage;
      _projectilePrefab = projectilePrefab;
      _victim = victim;
      _range = range;
      _impactPrefab = impactPrefab;
    }

    public override bool CanExecute()
    {
      var offenderPosition = Owner.transform.position;
      var victimPosition = _victim.transform.position;
      var difference = victimPosition - offenderPosition;
      var dir = difference.normalized;

      var ray = new Ray(offenderPosition + Vector3.up * 0.5f, dir);
      if (Physics.Raycast(ray, out var hit, Owner.weapon.range))
      {
        return hit.collider.gameObject == _victim.gameObject;
      }

      return false;
    }

    protected override void Start()
    {
      _projectile = Object.Instantiate(_projectilePrefab, Owner.transform.position, Quaternion.identity);
    }

    protected override void Process()
    {
      var transform = _projectile.transform;
      var position = transform.position;
      var targetPosition = _victim.transform.position;
      var difference = targetPosition - position;
      var distance = difference.sqrMagnitude;
      
      if (distance > 0.0001f)
      {
        transform.position = position + difference.normalized * Time.deltaTime * 25;
        transform.rotation = Quaternion.LookRotation(difference.normalized);
      }
      else
      {
        var forward = transform.forward;
        forward.y = 0;
        transform.rotation = Quaternion.LookRotation(forward.normalized);
        Finish();
      }
    }

    protected override void End()
    {
      var victimHealth = _victim.GetComponent<DamageableEntity>();
      if (victimHealth != null)
      {
        victimHealth.damageable.SufferDamage(_damage);
        Object.Destroy(_projectile);

        if (_impactPrefab != null)
        {
          Object.Instantiate(_impactPrefab, _victim.transform.position, Quaternion.identity);
        }
      }
    }
  }
}