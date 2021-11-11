using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityLogic.Abilities
{
  [Serializable]
  public class AbilityProcessor
  {
    public IAbility moveAbility = new ImplicitAbility();
    
    public List<AbilityBase> abilities = new List<AbilityBase>();

    public IAbility SelectedAbility { get; private set; }
    public int SelectedAbilityIndex { get; private set; }

    public event Action<IAbility, int> SelectedAbilityChanged;

    public AbilityProcessor()
    {
      DeselectAbility();
    }

    public AbilityBase GetAbility(int index)
    {
      return index >= 0 && index < abilities.Count ? abilities[index] : null;
    }

    public void SelectAbility(int index)
    {
      if (index >= 0 && index < abilities.Count)
      {
        SelectedAbilityIndex = index;
        SelectedAbility = abilities[index];
        SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
      }
      else
      {
        DeselectAbility();
      }
    }

    public bool SelectAbility(Type ability)
    {
      for (var i = 0; i < abilities.Count; i++)
      {
        if (abilities[i].GetType() != ability) continue;
        SelectedAbilityIndex = i;
        SelectedAbility = abilities[i];
        SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
        return true;
      }

      return false;
    }

    public void DeselectAbility()
    {
      if (SelectedAbilityIndex == -1)
      {
        return;
      }
      
      SelectedAbility = moveAbility;
      SelectedAbilityIndex = -1;
      SelectedAbilityChanged?.Invoke(SelectedAbility, SelectedAbilityIndex);
    }
  }
}