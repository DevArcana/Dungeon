using UnityEngine;

public class GridSelectionMap : MonoBehaviour
{
    public Transform highlightPrefab;
    private Grid<GridHighlight> _selection;

    public void InitGrid(int width, int height, float cellSize, Vector3 origin)
    {
        _selection = new Grid<GridHighlight>(width, height, cellSize, origin);
    }

    public void SetHighlight(Vector3 pos, bool value)
    {
        _selection.WorldToGrid(pos, out var x, out var y);

        if (value)
        {
            if (_selection[x, y] is null)
            {
                var position = _selection.GridToWorld(x, y) + new Vector3(0.5f, 0.0f, 0.5f);
                var highlight = Instantiate(highlightPrefab, position, Quaternion.identity, transform)
                    .GetComponent<GridHighlight>();

                highlight.AssignSelectionCallback(x, y, OnValueSelected);
                _selection[x, y] = highlight;

            }
        }
        else
        {
            if (!(_selection[x, y] is null))
            {
                var highlight = _selection[x, y];
                Destroy(highlight.gameObject);
                _selection[x, y] = null;
            }
        }
    }

    public bool IsHighlighted(Vector3 pos)
    {
        _selection.WorldToGrid(pos, out var x, out var y);
        return !(_selection[x, y] is null);
    }

    public void OnValueSelected(int x, int y, GridHighlight highlight)
    {
        
    }
}