using System;
using System.Collections.Generic;

namespace Abilities
{
  [Serializable]
  public class AbilityProcessor
  {
    public AbilityBase moveAbility;
    
    public AbilityBase ability1;
    public AbilityBase ability2;
    public AbilityBase ability3;
    public AbilityBase abilitySpecial;

    public AbilityBase selectedAbility;
    public int selectedAbilityNumber;
    
    public IEnumerable<AbilityBase> GetAvailableAbilities()
    {
      var abilities = new List<AbilityBase> { moveAbility, ability1, ability2, ability3, abilitySpecial };

      return abilities;
    }

    public void SelectAbility(int abilityNumber)
    {
      selectedAbilityNumber = abilityNumber;
      selectedAbility = abilityNumber switch
      {
        1 => ability1,
        2 => ability2,
        3 => ability3,
        4 => abilitySpecial,
        _ => throw new ArgumentOutOfRangeException(nameof(abilityNumber), abilityNumber, null)
      };
    }

    public void DeselectAbility()
    {
      selectedAbilityNumber = 0;
      selectedAbility = moveAbility;
    }
  }
}