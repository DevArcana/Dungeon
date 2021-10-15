using UnityEngine;

namespace EntityLogic
{
  [RequireComponent(typeof(GridEntity))]
  public class DamageableEntity : MonoBehaviour
  {
    private GridEntity _entity;
    public Damageable damageable = new Damageable();

    private void Start()
    {
      _entity = GetComponent<GridEntity>();
      damageable.EntityDied += OnDeath;
    }

    private void OnDestroy()
    {
      damageable.EntityDied -= OnDeath;
    }

    private void OnDeath()
    {
      World.World.instance.Unregister(_entity);
      Destroy(gameObject);
    }
  }
}