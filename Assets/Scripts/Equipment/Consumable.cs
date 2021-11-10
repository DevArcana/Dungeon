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
    }
}