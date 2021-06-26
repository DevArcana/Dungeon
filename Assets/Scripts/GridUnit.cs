using UnityEngine;

public class GridUnit : MonoBehaviour
{
    public GameBoard board;

    private void Start()
    {
        board.SelfRegister(this);
    }
}