using System;
using UnityEngine;

namespace Utils
{
  [Serializable]
  public class ReactiveVariable<T>
  {
    [SerializeField]
    private T value;

    public event Action<T, T> ValueChanged; 

    public T CurrentValue
    {
      get => value;
      set
      {
        var old = this.value;
        this.value = value;
        ValueChanged?.Invoke(old, value);
      }
    }

    public ReactiveVariable(T value = default)
    {
      this.value = value;
    }
  }
}