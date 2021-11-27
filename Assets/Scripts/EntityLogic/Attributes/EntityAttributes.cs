using System.Collections.Generic;

namespace EntityLogic.Attributes
{
    public class EntityAttributes
    {
        public float Strength { get; set; }
        public float Agility { get; set; }
        public float Focus { get; set; }
        public float Initiative { get; set; }
        public float DamageReduction { get; set; }
        public float WeaponDamage { get; set; }
        public float WeaponRange { get; set; }
        public float MaximumHealth { get; set; }
        
        public List<AttributeModifier> PermanentModifiers { get; }

        public EntityAttributes()
        {
            PermanentModifiers = new List<AttributeModifier>();
        }
    }
}
