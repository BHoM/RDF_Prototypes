using BH.Adapter;
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
        public static bool IsBHoMType(this Type t)
        {
            return typeof(IObject).IsAssignableFrom(t) || typeof(BHoMAdapter).IsAssignableFrom(t);
        }
    }
}
