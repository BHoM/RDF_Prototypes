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
        public static object DefaultValue(this PropertyInfo pInfo)
        {
            Type propertyInfoParentType = pInfo.DeclaringType;

            object defaultValue = pInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfoParentType) : null;

            return defaultValue;
        }
    }
}