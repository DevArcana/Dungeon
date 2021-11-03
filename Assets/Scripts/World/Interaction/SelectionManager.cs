using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using World.Common;

namespace World.Interaction
{
  public class SelectionManager : MonoBehaviour
  {
    public static SelectionManager instance;

    public HighlightedTile prefab;

    private Dictionary<GridPos, HighlightedTile> _selected;

    private void Awake()
    {
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Destroy(gameObject);
      }

      _selected = new Dictionary<GridPos, HighlightedTile>();
    }

    public void Clear()
    {
      _selected.Clear();
      var childCount = transform.childCount;
      while (childCount > 0)
      {
        childCount--;
        var obj = transform.GetChild(childCount).gameObject;

        if (Application.isEditor)
        {
          DestroyImmediate(obj);
        }
        else
        {
          Destroy(obj);
        }
      }
    }

    public bool Deselect(GridPos pos)
    {
      if (!_selected.ContainsKey(pos))
      {
        return false;
      }
      
      var tile = _selected[pos];
      Destroy(tile.gameObject);
      _selected.Remove(pos);

      return true;
    }

    public void Select(GridPos pos, Action<GridPos> onClick = null, Action<GridPos> hoverEnter = null)
    {
      Select(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f), onClick);
    }
    
    public void Select(GridPos pos, Color color, Action<GridPos> onClick = null)
    {
      var darken = color * 0.85f;
      darken.a = color.a;
      Select(pos, color, darken, onClick);
    }

    public void Select(GridPos pos, Color color, Color hoverColor, Action<GridPos> onClick = null)
    {
      Deselect(pos);
      var tile = Instantiate(prefab, MapUtils.ToWorldPos(pos), Quaternion.identity, transform);
      tile.color = color;
      tile.hoverColor = hoverColor;
      tile.onClick = onClick;
      _selected[pos] = tile;
    }
  }
}