using System;
using System.Collections.Generic;
using UI;


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
        public Necklace necklace;
        public Ring ring;
        public Gloves gloves;
        public List<Item> backpack;
        
        public void Equip(Item item)
        {
            switch (item)
            {
                case Weapon w:
                {
                    if(!(weapon is null)) backpack.Add(weapon);
                    weapon = w;
                    backpack.Remove(w);
                    break;
                }
                case Helmet w:
                {
                    if(!(helmet is null)) backpack.Add(helmet);
                    helmet = w;
                    backpack.Remove(w);
                    break;
                }
                case Breastplate w:
                {
                    if(!(breastplate is null)) backpack.Add(breastplate);
                    breastplate = w;
                    backpack.Remove(w);
                    break;
                }
                case Leggings w:
                {
                    if(!(leggings is null)) backpack.Add(leggings);
                    leggings = w;
                    backpack.Remove(w);
                    break;
                }
                case Gloves w:
                {
                    if(!(gloves is null)) backpack.Add(gloves);
                    gloves = w;
                    backpack.Remove(w);
                    break;
                }
                case Boots w:
                {
                    if(!(boots is null)) backpack.Add(boots);
                    boots = w;
                    backpack.Remove(w);
                    break;
                }
                case Ring w:
                {
                    if(!(ring is null)) backpack.Add(ring);
                    ring = w;
                    backpack.Remove(w);
                    break;
                }
                case Necklace w:
                {
                    if(!(necklace is null)) backpack.Add(necklace);
                    necklace = w;
                    backpack.Remove(w);
                    break;
                }
            }

            EquipmentUI.iconsGenerated = false;
            EquipmentUI.isItemDescriptionEnabled = false;
        }
        
        public void UnEquip(Item item)
        {
            switch (item)
            {
                case Weapon w:
                {
                    backpack.Add(weapon);
                    weapon = null;
                    break;
                }
                case Helmet w:
                {
                    backpack.Add(helmet);
                    helmet = null;
                    break;
                }
                case Breastplate w:
                {
                    backpack.Add(breastplate);
                    breastplate = null;
                    break;
                }
                case Leggings w:
                {
                    backpack.Add(leggings);
                    leggings = null;
                    break;
                }
                case Gloves w:
                {
                    backpack.Add(gloves);
                    gloves = null;
                    break;
                }
                case Boots w:
                {
                    backpack.Add(boots);
                    boots = null;
                    break;
                }
                case Ring w:
                {
                    backpack.Add(ring);
                    ring = null;
                    break;
                }
                case Necklace w:
                {
                    backpack.Add(necklace);
                    necklace = null;
                    break;
                }
            }
            EquipmentUI.iconsGenerated = false;
            EquipmentUI.isItemDescriptionEnabled = false;
        }

        public void RemoveItem(Item item, bool isEquiped)
        {
            if (isEquiped)
            {
                UnEquip(item);
            }

            backpack.Remove(item);
            EquipmentUI.iconsGenerated = false;
            EquipmentUI.isItemDescriptionEnabled = false;
        }
    }
}