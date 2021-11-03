using System;
using UnityEngine;
using Utils;
using World.Common;

namespace World.Interaction
{
  public class HighlightedTile : MonoBehaviour
  {
    public new Renderer renderer;

    public Color color = Color.white;
    public Color hoverColor = Color.grey;

    public Action<GridPos> onClick;

    private void Start()
    {
      renderer.material.color = color;
    }

    private void OnMouseEnter()
    {
      Debug.Log("enter");
      renderer.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
      Debug.Log("exit");
      renderer.material.color = color;
    }

    private void OnMouseUpAsButton()
    {
      onClick?.Invoke(MapUtils.ToMapPos(transform.position));
    }
  }
}