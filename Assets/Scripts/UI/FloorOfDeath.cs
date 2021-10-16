using TMPro;
using UnityEngine;

namespace UI
{
  public class FloorOfDeath : MonoBehaviour
  {
    private TextMeshProUGUI _text;

    private void Start()
    {
      _text = GetComponent<TextMeshProUGUI>();
      var floor = World.World.instance.currentFloor.CurrentValue - 1;
      _text.text = World.World.instance != null
        ? _text.text.Replace("<floor>", floor.ToString())
        : "How strange... Did this scene load from incorrect place?";
    }
  }
}