using System.Collections.Generic;
using UnityEngine;
using Utils;
using World.Common;

namespace World.Interaction
{
  public class SelectionManager : MonoBehaviour
  {
    public static SelectionManager instance;

    public GameObject targetPositionSelectionPrefab;
    public GameObject abilityRangeSelectionPrefab;

    private readonly Dictionary<GridPos, GameObject> _targetPositionLayerEnabled = new Dictionary<GridPos, GameObject>();
    private readonly List<GameObject> _targetPositionLayerDisabled = new List<GameObject>();
    
    private readonly Dictionary<GridPos, GameObject> _abilityRangeLayerEnabled = new Dictionary<GridPos, GameObject>();
    private readonly List<GameObject> _abilityRangeLayerDisabled = new List<GameObject>();
    
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

    public void ClearTargetPositions()
    {
      foreach (var tile in _targetPositionLayerEnabled.Values)
      {
        tile.SetActive(false);
        _targetPositionLayerDisabled.Add(tile);
      }

      _targetPositionLayerEnabled.Clear();
    }

    public void ClearAbilityRange()
    {
      foreach (var tile in _abilityRangeLayerEnabled.Values)
      {
        tile.SetActive(false);
        _abilityRangeLayerDisabled.Add(tile);
      }
      
      _abilityRangeLayerEnabled.Clear();
    }
    
    public void HighlightTargetPosition(GridPos pos, bool highlight = true)
    {
      var position = MapUtils.ToWorldPos(pos);
      if (highlight)
      {
        if (_targetPositionLayerDisabled.Count == 0)
        {
          _targetPositionLayerEnabled[pos] = Instantiate(targetPositionSelectionPrefab, position, Quaternion.identity, transform);
        }
        else
        {
          var tile = _targetPositionLayerDisabled[0];
          _targetPositionLayerDisabled.RemoveAt(0);
          _targetPositionLayerEnabled[pos] = tile;
          tile.SetActive(true);
          tile.transform.position = MapUtils.ToWorldPos(pos);
        }
      }
      else if (_targetPositionLayerEnabled.TryGetValue(pos, out var tile))
      {
        _targetPositionLayerEnabled.Remove(pos);
        tile.SetActive(false);
        _targetPositionLayerDisabled.Add(tile);
      }
    }

    public void HighlightAbilityRange(GridPos pos, bool highlight = true)
    {
      var position = MapUtils.ToWorldPos(pos);
      if (highlight)
      {
        if (_abilityRangeLayerDisabled.Count == 0)
        {
          _abilityRangeLayerEnabled[pos] = Instantiate(abilityRangeSelectionPrefab, position, Quaternion.identity, transform);
        }
        else
        {
          var tile = _targetPositionLayerDisabled[0];
          _abilityRangeLayerDisabled.RemoveAt(0);
          _abilityRangeLayerEnabled[pos] = tile;
          tile.SetActive(true);
          tile.transform.position = MapUtils.ToWorldPos(pos);
        }
      }
      else if (_abilityRangeLayerEnabled.TryGetValue(pos, out var tile))
      {
        _abilityRangeLayerEnabled.Remove(pos);
        tile.SetActive(false);
        _abilityRangeLayerDisabled.Add(tile);
      }
    }
  }
}