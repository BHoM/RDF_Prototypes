using BH.oM.Base;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.External.RDF
{
    public static partial class Query
    {
        [Description("Checks whether a type is a class that is a subtype of IObject or of BHoMObject (therefore, excluding the 'BHoMObject' class itself).")]
        public static bool IsBHoMSubtype(this Type t)
        {
            return t.IsClass && (typeof(BHoMObject).IsSubclassOf(t) || typeof(IObject).IsAssignableFrom(t)) && t != typeof(BHoMObject);
        }
    }
}
