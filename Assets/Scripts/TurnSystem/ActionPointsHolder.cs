using System;

namespace TurnSystem
{
  public class ActionPointsHolder
  {
    private int _actionPoints;
    private int _reservedActionPoints;
    
    /// <summary>
    /// Maximum amount of action points an entity can have per turn.
    /// </summary>
    public const int MaxActionPoints = 5;
    
    /// <summary>
    /// Event fired whenever the amount of action points changes.
    /// </summary>
    public event EventHandler<EventArgs> ActionPointsChanged;

    /// <summary>
    /// Current actual amount of action points still left.
    /// </summary>
    public int ActionPoints
    {
      get => _actionPoints;
      set
      {
        _actionPoints = value;
        ActionPointsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Amount of action points reserved by transactions.
    /// </summary>
    public int ReservedActionPoints
    {
      get => _reservedActionPoints;
      set
      {
        _reservedActionPoints = value;
        ActionPointsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Available unallocated action points still remaining.
    /// </summary>
    public int RemainingActionPoints => ActionPoints - ReservedActionPoints;
  }
}