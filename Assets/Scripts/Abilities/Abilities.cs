namespace Abilities
{
  public class Abilities
  {
    /// <summary>
    /// First ability determined by entity type.
    /// </summary>
    public AbilityBase Slot1 { get; set; }
    
    /// <summary>
    /// Second ability determined by entity type.
    /// </summary>
    public AbilityBase Slot2 { get; set; }
    
    /// <summary>
    /// Third ability determined by entity type.
    /// </summary>
    public AbilityBase Slot3 { get; set; }
    
    /// <summary>
    /// Special ability activated by having appropriate equipment.
    /// </summary>
    public AbilityBase Special { get; set; }
  }
}