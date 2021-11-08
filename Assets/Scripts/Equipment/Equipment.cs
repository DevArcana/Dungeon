using System;
using System.Collections.Generic;

namespace Equipment
{
    [Serializable]
    public class EntityEquipment
    {
        public Weapon weapon;
        public Helmet helmet;
        public Breastplate breastplate;
        public Leggings leggings;
        public Boots boots;
        public List<Item> backpack;
    }
}