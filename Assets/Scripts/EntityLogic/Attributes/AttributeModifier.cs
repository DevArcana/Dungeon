using System;

namespace EntityLogic.Attributes
{
    [Serializable]
    public class AttributeModifier
    {
        public Attribute attribute;
        public double value;
        public ModifierType type;
    }

    public enum ModifierType
    {
        Additive,
        Multiplicative
    }
}
