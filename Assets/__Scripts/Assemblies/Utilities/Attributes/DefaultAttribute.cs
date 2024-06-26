﻿using System;

namespace __Scripts.Assemblies.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DefaultAttribute : Attribute
    {
        public readonly object value;

        public DefaultAttribute(int value)
        {
            this.value = value;
        }
        
        public DefaultAttribute(float value)
        {
            this.value = value;
        }
        
        public DefaultAttribute(string value)
        {
            this.value = value;
        }
    }
}
