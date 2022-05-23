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
        public static Dictionary<Type, List<Type>> m_cachedBaseTypes = new Dictionary<Type, List<Type>>();

        [Description("Gets all implemented interfaces and any base type of a given type.")]
        public static List<Type> BaseTypes(this Type type)
        {
            List<Type> baseTypes = new List<Type>();

            if (type == null)
                return baseTypes;

            if (m_cachedBaseTypes.TryGetValue(type, out baseTypes))
                return baseTypes;
            else
                baseTypes = new List<Type>();

            baseTypes.AddRange(type.GetInterfaces());

            Type baseType = type.BaseType;
            if (baseType != null)
                baseTypes.Add(baseType);

            m_cachedBaseTypes[type] = baseTypes;

            return baseTypes;
        }
    }
}