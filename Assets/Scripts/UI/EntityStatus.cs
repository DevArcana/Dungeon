using EntityLogic;
using TMPro;
using UnityEngine;

namespace UI
{
  [RequireComponent(typeof(TextMeshProUGUI))]
  public class EntityStatus : MonoBehaviour
  {
    public DamageableEntity entity;
    private TextMeshProUGUI _tmp;

    private void Start()
    {
      _tmp = GetComponent<TextMeshProUGUI>();
      entity.damageable.HealthChanged += OnDamageableChanged;
      _tmp.text = $"Health: {entity.damageable.Health}/{entity.damageable.MaxHealth}";
    }

    private void OnDamageableChanged()
    {
      _tmp.text = $"Health: {entity.damageable.Health}/{entity.damageable.MaxHealth}";
    }
  }
}