using System;
using UnityEngine;

namespace World.Level.Common
{
  // How many hoops do I have to jump through today?
  [Serializable]
  public class Matrix<T>
  {
    [Serializable]
    private class MatrixRow
    {
      [SerializeField]
      public T[] cells;
    }

    [SerializeField]
    private MatrixRow[] rows;

    public Matrix(int w, int h)
    {
      rows = new MatrixRow[w];

      for (var i = 0; i < w; i++)
      {
        rows[i] = new MatrixRow() {cells = new T[h]};
      }
    }

    public T this[int x, int y]
    {
      get => rows[x].cells[y];
      set => rows[x].cells[y] = value;
    }
  }
}