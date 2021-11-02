using System;
using System.Collections.Generic;

namespace Equipment
{
    [Serializable]
    public class EntityEquipment
    {
        public Weapon weapon;
        public Armor helmet;
        public Armor breastplate;
        public Armor leggings;
        public Armor boots;
        public List<Item> backpack;
    }
}