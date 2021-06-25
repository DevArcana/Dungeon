using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public Transform floorPrefab;

    private Grid<int> _grid;
    private readonly List<Transform> _tiles = new List<Transform>();
    
    private void Start()
    {
        _grid = new Grid<int>(width, height, 1.0f, transform.position, (grid, x, y) => 0);
        _grid.OnGridChanged += (sender, args) => RebuildGrid();
        RebuildGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                _grid.SetValue(hit.point, 1);
            }
        }
    }

    private void RebuildGrid()
    {
        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }
        
        _tiles.Clear();

        foreach (var (x, y, tile) in _grid)
        {
            if (tile == 0)
            {
                var pos = _grid.GridToWorld(x, y) + new Vector3(0.5f, 0.0f, 0.5f);
                _tiles.Add(Instantiate(floorPrefab, pos, Quaternion.identity, transform));
            }
        }
    }
}