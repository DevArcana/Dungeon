using System;

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
            EntityEquipment.isItemDescriptionEnabled = false;
        }
    }
}