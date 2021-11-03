namespace Transactions
{
  /// <summary>
  /// Transaction base class. All transactions must inherit from it.
  /// </summary>
  /// <remarks>Do not reuse transactions. Each transaction should be constructed before each use.</remarks>
  public abstract class TransactionBase
  {
    private bool _finished;
    private bool _started;

    /// <summary>
    /// This method is called once at the beginning before processing the transaction.
    /// </summary>
    protected virtual void Start()
    {
      
    }

    /// <summary>
    /// This method is called each frame during Update unity event until 'Finish()' is invoked.
    /// </summary>
    /// <remarks>Do not call its base method as it simply calls 'Finish()'.</remarks>
    protected virtual void Process()
    {
      Finish();
    }

    /// <summary>
    /// This method is called once at the end after processing of the transaction, directly after calling 'Finish()'.
    /// </summary>
    protected virtual void End()
    {
      
    }

    /// <summary>
    /// This method is called by the Turn Manager each frame.
    /// </summary>
    /// <returns>A boolean indicating whether a given transaction has finished.</returns>
    public bool Run()
    {
      if (!_started)
      {
        Start();
        _started = true;
      }
      
      Process();

      if (_finished)
      {
        End();
        return true;
      }
      
      return false;
    }
    
    /// <summary>
    /// Call this method from within 'Process' to end execution of a given transaction.
    /// </summary>
    protected void Finish()
    {
      _finished = true;
    }
  }
}