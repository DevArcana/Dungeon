using Grid;

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
    /// The cost in action points required to process a given transaction.
    /// </summary>
    public int Cost { get; }
    
    /// <summary>
    /// The owner of a transaction used to check whether a transaction can be performed during their turn.
    /// </summary>
    /// <remarks>Can be null for entity invariant transactions such as global events.</remarks>
    public GridEntity Owner { get; }

    protected TransactionBase(int cost, GridEntity owner = null)
    {
      Cost = cost;
      Owner = owner;
    }

    /// <summary>
    /// Called directly before enqueuing the transaction to determine whether it's possible to execute it and also before execution.
    /// </summary>
    /// <returns>A boolean determining whether a transaction can be executed.</returns>
    public virtual bool CanExecute()
    {
      return true;
    }

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