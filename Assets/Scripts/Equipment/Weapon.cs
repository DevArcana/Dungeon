namespace Equipment
{
    public class Weapon : Item
    {
        public float baseDamage;
        public float baseRange;
        public bool isRanged;

        public override string AttributeNames()
        {
            return "Damage:\nRange:\n" + base.AttributeNames();
        }

        public override string AttributeValues()
        {
            return $"{baseDamage}\n{baseRange}\n" + base.AttributeValues();
        }
    }
}