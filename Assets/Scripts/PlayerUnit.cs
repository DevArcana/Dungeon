using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public GameBoard board;
    
    private Camera _camera;
    private GridUnit _unit;

    private void Start()
    {
        _unit = GetComponent<GridUnit>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out var hit))
            {
                var pos = board.Move(_unit, hit.point);
                if (pos.HasValue)
                {
                    transform.position = pos.Value + new Vector3(0.5f, 0.0f, 0.5f);
                }
            }
        }
    }
}