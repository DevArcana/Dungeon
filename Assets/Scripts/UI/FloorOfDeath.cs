using TMPro;
using UnityEngine;
using World;

namespace UI
{
  public class FloorOfDeath : MonoBehaviour
  {
    private TextMeshProUGUI _text;

    private void Start()
    {
      _text = GetComponent<TextMeshProUGUI>();
      var floor = CrossSceneContainer.instance.currentFloor.CurrentValue - 1;
      _text.text = _text.text.Replace("<floor>", floor.ToString());
    }
  }
}