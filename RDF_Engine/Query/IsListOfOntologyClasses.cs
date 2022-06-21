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
using BH.oM.Base;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static bool IsListOfOntologyClasses(this Type t)
        {
            if (typeof(IList).IsAssignableFrom(t))
            {
                Type[] genericArgs = t.GetGenericArguments();

                if (genericArgs.Length == 1 && genericArgs.First().IsOntologyClass())
                    return true;
            }

            return false;
        }
    }
}
