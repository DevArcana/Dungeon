using System;
using EntityLogic;

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

    public int SelectedAbilityNumber { get; private set; }

    public AbilityBase SelectedAbility => SelectedAbilityNumber switch
    {
      1 => Slot1,
      2 => Slot2,
      3 => Slot3,
      4 => Special,
      _ => null
    };

    public class AbilitySelectionChangedEventArgs : EventArgs
    {
      public int AbilityNumber { get; set; }
    }

    public event EventHandler<AbilitySelectionChangedEventArgs> AbilitySelectionChanged;

    private void OnAbilitySelectionChanged()
    {
      AbilitySelectionChanged?.Invoke(this, new AbilitySelectionChangedEventArgs {AbilityNumber = SelectedAbilityNumber});
    }

    public static Abilities FromEntity(GridLivingEntity entity)
    {
      return new Abilities(entity.ability1, entity.ability2, entity.ability3, entity.abilitySpecial);
    }

    public Abilities(AbilityBase slot1, AbilityBase slot2, AbilityBase slot3, AbilityBase special)
    {
      Slot1 = slot1;
      Slot2 = slot2;
      Slot3 = slot3;
      Special = special;
    }

    public void SelectAbility(int abilityNumber)
    {
      SelectedAbilityNumber = abilityNumber;
      OnAbilitySelectionChanged();
    }

    public void DeselectAbility()
    {
      SelectedAbilityNumber = 0;
      OnAbilitySelectionChanged();
    }
  }
}