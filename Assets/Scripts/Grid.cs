using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T> : IEnumerable<(int, int, T)>, IEnumerable
{
    private readonly int _width, _height;
    private readonly float _cellSize;
    private readonly T[,] _data;
    private readonly Vector3 _origin;

    public class OnGridChangedEventArgs : EventArgs
    {
        public int X;
        public int Y;
    }

    public event EventHandler<OnGridChangedEventArgs> OnGridChanged;
    
    public void NotifyGridChanged(int x, int y)
    {
        OnGridChanged?.Invoke(this, new OnGridChangedEventArgs {X = x, Y = y});
    }

    public Grid(int width, int height, float cellSize, Vector3 origin, Func<Grid<T>, int, int, T> defaultValueFunc = null)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _origin = origin;
        _data = new T[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _data[x, y] = defaultValueFunc is null ? default : defaultValueFunc(this, x, y);
            }
        }
    }

    public bool IsWithinGrid(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;

    public void WorldToGrid(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - _origin).x / _cellSize);
        y = Mathf.FloorToInt((worldPos - _origin).z / _cellSize);
    }

    public Vector3 GridToWorld(int x, int y) => new Vector3(x, 0, y) * _cellSize + _origin;

    public T GetValue(int x, int y)
    {
        if (!IsWithinGrid(x, y))
        {
            return default;
        }
        
        return _data[x, y];
    }
    
    public bool SetValue(int x, int y, T value)
    {
        if (!IsWithinGrid(x, y))
        {
            return false;
        }

        _data[x, y] = value;
        NotifyGridChanged(x, y);
        
        return true;
    }
    
    public bool SetValue(Vector3 pos, T value)
    {
        WorldToGrid(pos, out var x, out var y);
        return SetValue(x, y, value);
    }

    public T this[int x, int y]
    {
        get => GetValue(x, y);
        set => SetValue(x, y, value);
    }

    public IEnumerator<(int, int, T)> GetEnumerator()
    {
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                 yield return (x, y, _data[x, y]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
