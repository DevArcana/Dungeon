using System;
using UnityEngine;

public class GridDebugger : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public float cellSize = 1.0f;

    private Grid<bool> _grid;

    private Mesh _cellMesh;

    private void Start()
    {
        _grid = new Grid<bool>(width, height, cellSize, transform.position);
        
        _cellMesh = new Mesh
        {
            vertices = new[]
            {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f)
            },
            triangles = new[]
            {
                0, 1, 2,
                2, 3, 0
            }
        };
        
        _cellMesh.RecalculateNormals();
    }
    
    private void OnDrawGizmos()
    {
        foreach (var (x, y, _) in _grid)
        {
            var pos = _grid.GridToWorld(x, y);
            Gizmos.DrawWireMesh(_cellMesh, pos);
        }
    }
}