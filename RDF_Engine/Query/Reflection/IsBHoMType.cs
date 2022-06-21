using BH.Adapter;
using BH.Engine.RDF.Types;
using BH.oM.Base;
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
        [Description("Checks whether a type is a bhom type.")]
        public static bool IsBHoMType(this Type t, bool noGenericsDuplicates = true)
        {
            if (t == null || (noGenericsDuplicates && t.Name.StartsWith("<>c__")))
                return false;

            if (t == null)
                return false;

            // Guard against dynamically loaded assemblies from external locations
            if (!typeof(IObject).Module.FullyQualifiedName.Contains("C:\\ProgramData\\BHoM\\Assemblies") ||
                !typeof(BHoMAdapter).Module.FullyQualifiedName.Contains("C:\\ProgramData\\BHoM\\Assemblies"))
                return (t.FullName?.StartsWith("BH.oM.") ?? false) || (t.FullName?.StartsWith(typeof(CustomObjectType).FullName) ?? false);

            return typeof(IObject).IsAssignableFrom(t) || typeof(BHoMAdapter).IsAssignableFrom(t);
        }
    }
}
