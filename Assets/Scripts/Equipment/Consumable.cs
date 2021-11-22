using System;
using UI;

namespace Equipment
{
    public class Consumable : Item
    {
        public Consumable()
        {
            itemRarity = ItemRarity.Common;
            ability = null;
        }

        public virtual void Use()
        {
            EquipmentUI.isItemDescriptionEnabled = false;
        }
    }
}