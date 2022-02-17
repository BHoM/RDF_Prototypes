using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static HashSet<string> Namespaces(this IEnumerable<Type> types)
        {
            // Remove duplicate classes in the same file, e.g. `BH.oM.Base.Output` which has many generics replicas.
            HashSet<string> namespaces = new HashSet<string>(types.Select(t => t.Namespace).Distinct());
            return namespaces;
        }
    }
}