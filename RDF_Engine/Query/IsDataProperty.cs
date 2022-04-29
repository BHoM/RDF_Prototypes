using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static bool IsDataProperty(this Type type, LocalRepositorySettings settings = null)
        {
            if (type.IsNumeric() || type == typeof(string))
                return true;

            return false;
        }
    }
}
