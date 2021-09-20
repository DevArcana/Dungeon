using System;
using Combat;
using TMPro;
using UnityEngine;

namespace UI
{
  [RequireComponent(typeof(TextMeshProUGUI))]
  public class EntityStatus : MonoBehaviour
  {
    public Health health;
    private TextMeshProUGUI _tmp;

    private void Start()
    {
      _tmp = GetComponent<TextMeshProUGUI>();
      health.HealthChanged += OnHealthChange;
      _tmp.text = $"Health: {health.health}/{health.maxHealth}";
    }

    private void OnHealthChange(object sender, Health.HealthChangedEventArgs e)
    {
      _tmp.text = $"Health: {e.health}/{health.maxHealth}";
    }
  }
}