namespace VRTK
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class VRTK_CustomLayoutSourceSelectorAttribute : Attribute
    {
        public Type sourceType;

        public VRTK_CustomLayoutSourceSelectorAttribute(Type type)
        {
            sourceType = type;
        }
    }
}