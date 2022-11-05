using MyORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyORM
{
    public static class EntityModel
    {
        public static string GetName<T>() => GetName(typeof(T));

        public static string GetName(Type type) =>
            Attribute.IsDefined(type, typeof(TableAttribute)) 
                ? type.GetCustomAttribute<TableAttribute>().Name 
                : type.Name;
                

        public static string GetColumnName(PropertyInfo property) =>
            Attribute.IsDefined(property, typeof(NameOverrideAttribute)) 
                ? property.GetCustomAttribute<NameOverrideAttribute>().Name 
                : property.Name;

        public static IEnumerable<PropertyInfo> GetVisibleProperties<T>() =>
            typeof(T).GetProperties()
                .Where(x => !Attribute.IsDefined(x, typeof(HideAttribute)));

        public static IEnumerable<PropertyInfo> GetEditableProperties<T>() =>
            typeof(T).GetProperties()
                .Where(p => !Attribute.IsDefined(p, typeof(ImmutableAttribute)) &&
                            !Attribute.IsDefined(p, typeof(HideAttribute)));

        public static PropertyInfo GetKeyColumn<T>() =>
            GetVisibleProperties<T>()
                .SingleOrDefault(x => Attribute.IsDefined(x, typeof(KeyAttribute)));
    }
}
