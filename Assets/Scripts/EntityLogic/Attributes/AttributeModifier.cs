namespace EntityLogic.Attributes
{
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
