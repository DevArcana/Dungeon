using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  [ExecuteInEditMode]
  public class Tooltip : MonoBehaviour
  {
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;

    public LayoutElement layoutElement;
    public int characterLimit = 200;

    private void Update()
    {
      layoutElement.enabled = Math.Max(header.text.Length, content.text.Length) > characterLimit;
      transform.position = Input.mousePosition + new Vector3(8, 8);
    }
  }
}