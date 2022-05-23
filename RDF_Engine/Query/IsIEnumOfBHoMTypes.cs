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
        public static bool IsIEnumOfBHoMTypes(this Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Count() == 1 && genericArgs.FirstOrDefault().IsBHoMType())
                return true;

            return false;
        }
    }
}
