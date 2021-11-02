using System.Collections.Generic;
using UnityEngine;
using Utils;
using World.Common;

namespace World.Interaction
{
  public class SelectionManager : MonoBehaviour
  {
    public static SelectionManager instance;

    public GameObject selectionPrefab;

    private readonly List<GameObject> _disabled = new List<GameObject>();
    private readonly Dictionary<GridPos, GameObject> _enabled = new Dictionary<GridPos, GameObject>();
    
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
    }

    public void Clear()
    {
      foreach (var enabled in _enabled.Values)
      {
        enabled.SetActive(false);
        _disabled.Add(enabled);
      }
      
      _enabled.Clear();
    }

    public void Highlight(GridPos pos, bool highlight = true)
    {
      var position = MapUtils.ToWorldPos(pos);
      if (highlight)
      {
        if (_disabled.Count == 0)
        {
          _enabled[pos] = Instantiate(selectionPrefab, position, Quaternion.identity, transform);
        }
        else
        {
          var tile = _disabled[0];
          _disabled.RemoveAt(0);
          _enabled[pos] = tile;
          tile.SetActive(true);
          tile.transform.position = MapUtils.ToWorldPos(pos);
        }
      }
      else if (_enabled.TryGetValue(pos, out var tile))
      {
        _enabled.Remove(pos);
        tile.SetActive(false);
        _disabled.Add(tile);
      }
    }
  }
}