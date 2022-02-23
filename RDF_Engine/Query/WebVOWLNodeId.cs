using BH.Engine.Base;
using BH.oM.RDF;
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
        public static string WebVOWLNodeId(this Type type)
        {
            return type.FullName;
        }

        public static string WebVOWLNodeId(this TypeInfo typeInfo)
        {
            return typeInfo.FullName;
        }

        public static string WebVOWLNodeId(this PropertyInfo propertyInfo)
        {
            return $"{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}";
        }

        public static string WebVOWLNodeId(this IRelation relation)
        {
            return $"{(relation.Subject as dynamic)?.Name ?? relation.Subject.GetType().Name}-{relation.GetType().Name}-{(relation.Object as dynamic)?.Name ?? relation.Object.GetType().Name}_{relation.Hash()}";
        }
    }
}
