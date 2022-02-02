using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Get the most nested type within a generic type. For example, passing a type of `List<List<Bar>>` will return the type of `Bar`." +
            "Supports only generic types and IEnumerables." +
            "If the generic type owns more than one generic argument, returns only the first (e.g. for a key-value collection it will only return the type of the key).")]
        public static Type InnermostType<T>(T obj)
        {
            Type type = typeof(T);

            if (type == typeof(System.Type) && obj is Type && obj != null)
                type = obj as Type;

            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                    return InnermostType(genericArgs[0]);
            }
            else
            {
                if (obj is IEnumerable)
                {
                    return InnermostType(
                        obj.GetType()
                        .GetInterfaces()
                        .Where(t => t.IsGenericType
                            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Select(t => t.GetGenericArguments()[0]).FirstOrDefault());
                }
            }

            return type;
        }
    }
}
