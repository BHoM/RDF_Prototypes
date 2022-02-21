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
        [Description("If the input type is a Generic type, looks recurively in its generic Arguments to see if a BHoM type can be found." +
            "I.e. if any argument is a generic argument, recurses until a non-generic argument is found, which is tested for being a BHoM type.")]
        public static bool IsGenericTypeWithBHoMArgs(this Type t)
        {
            if (!t.IsGenericType)
                return false;

            return RecurseGenericsForBHoMArg(t);
        }

        private static bool RecurseGenericsForBHoMArg(this Type t)
        {
            if (t.IsBHoMType())
                return true;

            List<Type> genericArgs = t.GetGenericArguments().ToList();

            foreach (Type type in genericArgs)
            {
                if (type.RecurseGenericsForBHoMArg())
                    return true;
            }

            return false;
        }
    }
}
