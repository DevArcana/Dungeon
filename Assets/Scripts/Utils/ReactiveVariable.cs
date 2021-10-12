using System;
using UnityEngine;

namespace Utils
{
  public class ReactiveVariableEventArgs<T> : EventArgs
  {
    public readonly T old;
    public readonly T current;

    public ReactiveVariableEventArgs(T old, T current)
    {
      this.old = old;
      this.current = current;
    }
  }
  
  [Serializable]
  public class ReactiveVariable<T>
  {
    [SerializeField]
    private T value;

    public event EventHandler<ReactiveVariableEventArgs<T>> ValueChanged; 

    public T CurrentValue
    {
      get => value;
      set
      {
        var args = new ReactiveVariableEventArgs<T>(this.value, value);
        this.value = value;
        ValueChanged?.Invoke(this, args);
      }
    }

    public ReactiveVariable(T value = default)
    {
      this.value = value;
    }
  }
}