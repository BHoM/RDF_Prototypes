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
        public static bool? IsInNamespace(this Type type, string nameSpace)
        {
            if (string.IsNullOrWhiteSpace(nameSpace))
                return null;

            return type.Namespace.StartsWith(nameSpace);
        }

        public static bool? IsInNamespace(this Type type, IEnumerable<string> nameSpaces)
        {
            if (nameSpaces == null || !nameSpaces.Any() || nameSpaces.Any(ns => string.IsNullOrWhiteSpace(ns)))
                return null;

            bool res = nameSpaces.Any(ns => type.Namespace.StartsWith(ns));

            return res;
        }
    }
}