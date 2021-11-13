using System;

namespace Equipment
{
    public class Consumable : Item
    {
        public Consumable()
        {
            itemRarity = ItemRarity.Common;
            level = 1;
            ability = null;
        }

        public void Use()
        {
            //Todo
            EntityEquipment.isItemDescriptionEnabled = false;
        }
    }
}