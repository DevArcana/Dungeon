using System;

namespace Equipment
{
    public enum ItemRarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Mythic
    }
    [Serializable]
    public class Item
    {
        public int level;
        public ItemRarity itemRarity;
    }
}