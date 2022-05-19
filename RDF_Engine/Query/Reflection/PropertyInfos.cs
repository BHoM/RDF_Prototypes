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
        public static PropertyInfo[] PropertyInfos(this Type type, bool declaredOnly = true)
        {
            PropertyInfo[] pInfos;

            if (m_cachedPropertyInfosPerType.TryGetValue(type, out pInfos))
                return pInfos;

            BindingFlags bindingFlags = BindingFlags.Public;

            if (declaredOnly) bindingFlags = bindingFlags | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly;

            pInfos = type.GetProperties(bindingFlags);

            m_cachedPropertyInfosPerType[type] = pInfos;

            return pInfos;
        }

        private static Dictionary<Type, PropertyInfo[]> m_cachedPropertyInfosPerType = new Dictionary<Type, PropertyInfo[]>();
    }
}
