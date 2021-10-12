using System;
using TurnSystem;
using UnityEngine;

namespace UI
{
  public class TurnsUI : MonoBehaviour
  {
    public Portrait portraitPrefab;
    public int queueSize = 7;

    private void Start()
    {
      Destroy(transform.GetChild(0).gameObject);
      TurnManager.instance.TurnEntityAdded += OnTurnEntityAdded;
      TurnManager.instance.TurnChanged += OnTurnChanged;
    }

    private void OnDestroy()
    {
      TurnManager.instance.TurnEntityAdded -= OnTurnEntityAdded;
      TurnManager.instance.TurnChanged -= OnTurnChanged;
    }

    private void Rebuild()
    {
      foreach (Transform child in transform)
      {
        Destroy(child.gameObject);
      }
      
      var queue = TurnManager.instance.PeekQueue(queueSize);

      foreach (var entity in queue)
      {
        var portrait = Instantiate(portraitPrefab, transform);
        portrait.image.sprite = entity.portrait;
        portrait.text.text = entity.entityName;
      }
    }
    
    private void OnTurnEntityAdded(object sender, TurnManager.TurnEventArgs args)
    {
      Rebuild();
    }
    
    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs args)
    {
      Rebuild();
    }
  }
}