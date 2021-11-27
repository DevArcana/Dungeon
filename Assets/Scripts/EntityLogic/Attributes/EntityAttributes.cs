using System.Collections.Generic;
using System.Reflection;

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
        
        public List<AttributeModifier> PermanentModifiers { get; }

        public EntityAttributes()
        {
            PermanentModifiers = new List<AttributeModifier>();
        }

        public string AttributeNames()
        {
            return "Strength:\nAgility:\nFocus:\nInitiative:\nDamage Reduction:\nDamage:\nRange:\nMax Health:\nCurrent Health:";
        }

        public string AttributeValues()
        {
            return
                $"{Strength}\n{Agility}\n{Focus}\n{Initiative}\n{DamageReduction}\n{WeaponDamage}\n{WeaponRange}\n";
        }
    }
}
