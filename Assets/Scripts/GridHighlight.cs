using System;
using UnityEngine;

public class GridHighlight : MonoBehaviour
{
    private Color _regularColor;
    private Color _hoverColor;

    private Color _targetColor;

    private Material _material;
    
    private Action<int, int, GridHighlight> _callback;
    private int _x, _y;

    private void Start()
    {
        _material = GetComponentInChildren<Renderer>().material;

        _regularColor = _material.color;
        _hoverColor = _regularColor + new Color(0.1f, 0.1f, 0.1f);

        _targetColor = _regularColor;
    }

    private void Update()
    {
        _material.color = Color.Lerp(_material.color, _targetColor, Time.deltaTime * 10);
    }

    private void OnMouseEnter()
    {
        _targetColor = _hoverColor;
    }

    private void OnMouseExit()
    {
        _targetColor = _regularColor;
    }

    public void AssignSelectionCallback(int x, int y, Action<int, int, GridHighlight> callback)
    {
        _x = x;
        _y = y;
        _callback = callback;
    }

    private void OnMouseDown()
    {
        _callback(_x, _y, this);
    }
}