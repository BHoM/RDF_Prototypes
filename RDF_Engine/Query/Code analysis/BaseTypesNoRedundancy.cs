using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Dictionary<Type, List<Type>> m_cachedBaseTypesNoRedundancy = new Dictionary<Type, List<Type>>();

        [Description("Gets all implemented interfaces and any base type of a given type.\n" +
            "For parent types that implement types that also the child type implements, only the parent type is reported.")]
        public static List<Type> BaseTypesNoRedundancy(this Type sourceType)
        {
            List<Type> baseTypesNoRedundancy = null;
            if (m_cachedBaseTypesNoRedundancy.TryGetValue(sourceType, out baseTypesNoRedundancy))
                return baseTypesNoRedundancy;

            List<Type> baseTypes = sourceType.GetInterfaces().ToList();
            if (sourceType.BaseType != null)
            {
                baseTypes.Add(sourceType.BaseType);
                baseTypes.AddRange(sourceType.BaseType.GetInterfaces());
            }

            // Remove duplicates collected so far
            baseTypes = baseTypes.Distinct().ToList();

            baseTypesNoRedundancy = new List<Type>(baseTypes);

            foreach (Type baseType in baseTypes)
            {
                var baseTypeInterfaces = baseType.GetInterfaces();
                foreach (var baseTypeInterface in baseTypeInterfaces)
                {
                    baseTypesNoRedundancy.Remove(baseTypeInterface);
                }
            }

            m_cachedBaseTypesNoRedundancy[sourceType] = baseTypesNoRedundancy;

            return baseTypesNoRedundancy;
        }

    }
}
