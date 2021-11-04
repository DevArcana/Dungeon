using System;
using System.Collections.Generic;
using EntityLogic.Abilities;
using TurnSystem;
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
    private AbilityProcessor _abilityProcessor;
    private UnityEngine.Camera _camera;

    private void Awake()
    {
      _camera = UnityEngine.Camera.main;
      if (instance == null)
      {
        instance = this;
      }
      else if (instance != this)
      {
        Destroy(gameObject);
      }

      _selected = new Dictionary<GridPos, HighlightedTile>();
      Clear();

      TurnManager.instance.TurnChanged += OnTurnChange;
    }

    private void OnDestroy()
    {
      if (_abilityProcessor != null)
      {
        _abilityProcessor.SelectedAbilityChanged -= OnAbilityChange;
      }
    }

    private void OnTurnChange(object sender, TurnManager.TurnEventArgs e)
    {
      Clear();

      if (_abilityProcessor != null)
      {
        _abilityProcessor.SelectedAbilityChanged -= OnAbilityChange;
        _abilityProcessor = null;
      }
      
      var entity = e.Entity;

      if (!(entity is PlayerEntity))
      {
        // TODO: disabled for debug purposes
        // return;
      }
      
      _abilityProcessor = entity.abilities;
      _abilityProcessor.SelectedAbilityChanged += OnAbilityChange;
      OnAbilityChange(_abilityProcessor.SelectedAbility, _abilityProcessor.SelectedAbilityIndex);
    }

    private void OnAbilityChange(IAbility ability, int index)
    {
      Clear();

      foreach (var pos in ability.GetValidTargetPositions())
      {
        Select(pos);
      }
    }

    private void Update()
    {
      if (_abilityProcessor == null)
      {
        return;
      }

      if (TurnManager.instance.Transactions.HasPendingTransactions)
      {
        return;
      }
      
      // TODO: limit to correct layer
      if (Input.GetMouseButtonDown(0) && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
      {
        Debug.Log("ping");
        var pos = MapUtils.ToMapPos(hit.point);
        if (_selected.ContainsKey(pos))
        {
          Debug.Log("pong");
          _abilityProcessor.SelectedAbility.Execute(pos);
        }
      }
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

    public void Select(GridPos pos)
    {
      Select(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
    }
    
    public void Select(GridPos pos, Color color)
    {
      var darken = color * 0.85f;
      darken.a = color.a;
      Select(pos, color, darken);
    }

    public void Select(GridPos pos, Color color, Color hoverColor)
    {
      Deselect(pos);
      var tile = Instantiate(prefab, MapUtils.ToWorldPos(pos), Quaternion.identity, transform);
      tile.color = color;
      tile.hoverColor = hoverColor;
      _selected[pos] = tile;
    }
  }
}