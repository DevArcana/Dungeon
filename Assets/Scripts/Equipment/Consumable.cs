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

        public void Use()
        {
            //Todo
            EquipmentUI.isItemDescriptionEnabled = false;
        }
    }
}