using BH.Engine.Base;
using BH.Engine.RDF.Types;
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
        public static string UniqueNodeId(this object obj)
        {
            return _UniqueNodeId(obj as dynamic);
        }

        public static string _UniqueNodeId(this ListPropertyType type)
        {
            return type.RDFTypeName;
        }

        public static string _UniqueNodeId(this Type type)
        {
            return type.FullName;
        }

        public static string _UniqueNodeId(this TypeInfo typeInfo)
        {
            // Custom Type exception.
            if (typeInfo.AsType() is CustomObjectType)
                return UniqueNodeId(typeInfo.AsType());

            return typeInfo.FullName;
        }

        public static string _UniqueNodeId(this PropertyInfo propertyInfo)
        {
            return $"{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name}";
        }

        public static string _UniqueNodeId(this IRelation relation)
        {
            return $"{(relation.Subject as dynamic)?.Name ?? relation.Subject.GetType().Name}-{relation.GetType().Name}-{(relation.Object as dynamic)?.Name ?? relation.Object.GetType().Name}_{relation.Hash()}";
        }
    }
}
