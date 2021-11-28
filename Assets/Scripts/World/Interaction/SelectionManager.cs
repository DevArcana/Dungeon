using System.Collections.Generic;
using EntityLogic;
using EntityLogic.Abilities;
using TurnSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using World.Common;

namespace World.Interaction
{
  public class SelectionManager : MonoBehaviour
  {
    public static SelectionManager instance;

    public HighlightedTile prefab;

    private Dictionary<GridPos, HighlightedTile> _selected;
    private HashSet<GridPos> _availableTargets;
    private UnityEngine.Camera _camera;
    private GridPos? _hoverPos;

    private void Awake()
    {
      _camera = UnityEngine.Camera.main;
      if (instance == null)
      {
        instance = this;
        
        _selected = new Dictionary<GridPos, HighlightedTile>();
        _availableTargets = new HashSet<GridPos>();

        TurnManager.instance.TurnChanged += OnTurnChanged;
        TurnManager.instance.Transactions.TransactionsProcessed += OnTransactionsProcessed;
        AbilityProcessor.instance.SelectedAbilityChanged += OnSelectedAbilityChanged;
      
        Refresh();
      }
      else if (instance != this)
      {
        Destroy(gameObject);
      }
    }

    private void OnDestroy()
    {
      TurnManager.instance.TurnChanged -= OnTurnChanged;
      TurnManager.instance.Transactions.TransactionsProcessed -= Refresh;
      AbilityProcessor.instance.SelectedAbilityChanged -= OnAbilityChange;
    }

    private void OnTurnChanged(object sender, TurnManager.TurnEventArgs e)
    {
      Refresh();
    }

    private void OnTransactionsProcessed()
    {
      Refresh();
    }
    
    private void OnSelectedAbilityChanged(AbilityBase arg1, int arg2)
    {
      Refresh();
    }

    private void Refresh()
    {
      TurnManager.instance.ActionPoints.ReservePoints(0);
      Clear();
      
      if (!(TurnManager.instance.CurrentTurnTaker is PlayerEntity))
      {
        return;
      }
      
      var abilityProcessor = AbilityProcessor.instance;
      OnAbilityChange(abilityProcessor.SelectedAbility, abilityProcessor.SelectedAbilityIndex);
    }

    private void OnAbilityChange(AbilityBase ability, int index)
    {
      foreach (var pos in ability.GetValidTargetPositions())
      {
        _availableTargets.Add(pos);
        Select(pos);
      }

      if (_hoverPos.HasValue)
      {
        TurnManager.instance.ActionPoints.ReservePoints(ability.GetEffectiveCost(_hoverPos.Value));
        
        foreach (var pos in ability.GetEffectiveRange(_hoverPos.Value))
        {
          var occupant = World.instance.GetOccupant(pos);
          Select(pos, occupant is EnemyEntity ? Color.red : Color.green);
        }
      }
    }

    private void Update()
    {
      if (!(TurnManager.instance.CurrentTurnTaker is PlayerEntity))
      {
        return;
      }
      
      if (TurnManager.instance.Transactions.HasPendingTransactions)
      {
        return;
      }

      if (_camera == null)
      {
        _camera = UnityEngine.Camera.main;
      }
      
      if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit, float.MaxValue, layerMask: LayerMask.GetMask("Selections")))
      {
        var pos = MapUtils.ToMapPos(hit.point);

        if (_availableTargets.Contains(pos))
        {
          if (_hoverPos != pos)
          {
            _hoverPos = pos;
            Refresh();
          }
        }
        else
        {
          _hoverPos = null;
          Refresh();
        }
      }
      else if (_hoverPos.HasValue)
      {
        _hoverPos = null;
        Refresh();
      }

      var abilityProcessor = AbilityProcessor.instance;
      if (Input.GetMouseButtonDown(0) && _hoverPos.HasValue && TurnManager.instance.CurrentTurnTaker is PlayerEntity && abilityProcessor.CanExecute(_hoverPos.Value))
      {
        Clear();
        abilityProcessor.Execute(_hoverPos!.Value);
        _hoverPos = null;
      }

      if (Input.GetMouseButtonDown(1) && _hoverPos.HasValue)
      {
        Debug.Log(_hoverPos);
      }
    }

    public void Clear()
    {
      _selected.Clear();
      _availableTargets.Clear();
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
      Select(pos, World.instance.IsOccupied(pos) ? new Color(1.0f, 0.5f, 0.0f, 0.5f) : new Color(1.0f, 1.0f, 1.0f, 0.5f));
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