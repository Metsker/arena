using System;

namespace Assemblies.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        public readonly object value;

        public DefaultValueAttribute(int value)
        {
            this.value = value;
        }
        
        public DefaultValueAttribute(float value)
        {
            this.value = value;
        }
        
        public DefaultValueAttribute(string value)
        {
            this.value = value;
        }
    }
}
