using Grid;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public int width = 9;
    public int height = 9;

    public float cellSize = 1.0f;

    private class GridCell
    {
        public int X;
        public int Y;
        public Grid<GridCell> Grid;
        public GridUnit Unit;
        
        public GridCell(Grid<GridCell> grid, int x, int y)
        {
            X = x;
            Y = y;
            Grid = grid;
            Unit = null;
        }
    }
    
    private Grid<GridCell> _grid;

    private void Start()
    {
        _grid = new Grid<GridCell>(width, height, cellSize, transform.position, (grid, x, y) => new GridCell(grid, x, y));
    }

    public void SelfRegister(GridUnit unit)
    {
        _grid.WorldToGrid(unit.transform.position, out var x, out var y);
        _grid[x, y].Unit = unit;
        _grid.NotifyGridChanged(x, y);
    }

    public Vector3? Move(GridUnit unit, Vector3 pos)
    {
        if (!_grid.IsWithinGrid(pos))
        {
            return null;
        }
        
        _grid.WorldToGrid(unit.transform.position, out var x, out var y);
        _grid[x, y].Unit = null;
        _grid.WorldToGrid(pos, out x, out y);
        _grid[x, y].Unit = unit;
        return _grid.GridToWorld(x, y);
    }
}