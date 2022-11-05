using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class PropertyAttribute : Attribute { }

    public class ImmutableAttribute : PropertyAttribute { }
    public class HideAttribute : PropertyAttribute { }

    public class NameOverrideAttribute : PropertyAttribute
    {
        public string Name { get; }

        public NameOverrideAttribute(string name)
        {
            Name = name;
        }
    }
    public class RequiredAttribute : PropertyAttribute { }
    public class UniqueAttribute : PropertyAttribute { }
    public class KeyAttribute : PropertyAttribute { }
    public class DefaultAttribute : PropertyAttribute 
    { 
        public object DefaultValue { get; }

        public DefaultAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
    public class IdentityAttribute : ImmutableAttribute
    {
        public int Seed { get; }
        public int Step { get; }

        public IdentityAttribute(int seed = 1, int step = 1)
        {
            Seed = seed;
            Step = step;
        }
    }
    public class CheckAttribute : PropertyAttribute
    {
        public string Expression { get; }

        public CheckAttribute(string expression)
        {
            Expression = expression;
        }
    }
    public class ReferenceAttribute : PropertyAttribute
    {
        public Type Type { get; }
        public PropertyInfo Property { get; }

        public ReferenceAttribute(Type type, PropertyInfo property)
        {
            Type = type;
            Property = property;
        }
    }

    #region String settings

    public class MaxLengthAttribute : PropertyAttribute
    {
        public int Max { get; }

        public MaxLengthAttribute(int max)
        {
            Max = max;
        }
    }

    #endregion
}
