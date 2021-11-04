using UnityEngine;

namespace World.Interaction
{
  public class HighlightedTile : MonoBehaviour
  {
    public new Renderer renderer;

    public Color color = Color.white;
    public Color hoverColor = Color.grey;

    private void Start()
    {
      renderer.material.color = color;
    }

    private void OnMouseEnter()
    {
      renderer.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
      renderer.material.color = color;
    }
  }
}