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
        public static bool IsCollectionOfBHoMTypes(this Type type)
        {
            bool isCollectionOfBHoMTypes = false;
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                // We consider the type to be "a collection of BHoM Types" if:
                // - it is a IEnumerable
                // - it has exactly one generic argument that is a BHoM Type
                // NOTE: This fails to consider complex types like Dictionary<string, List<IObject>>. TODO: Improve.
                List<Type> genericBHoMArgs = type.GetGenericArguments().Where(t => t.IsBHoMType()).ToList();
                isCollectionOfBHoMTypes = genericBHoMArgs.Count == 1;
            }

            return isCollectionOfBHoMTypes;
        }
    }
}