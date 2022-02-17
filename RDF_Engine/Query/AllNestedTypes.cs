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
        public static HashSet<Type> AllNestedTypes(this List<Type> types)
        {
            return new HashSet<Type>(types.SelectMany(t => t.AllNestedTypes()));
        }

        public static HashSet<Type> AllNestedTypes(this Type type)
        {
            HashSet<Type> result = new HashSet<Type>();

            AllNestedTypes(type, result);

            return result;
        }

        private static void AllNestedTypes(this Type type, HashSet<Type> result)
        {
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                Type propertyType = prop.PropertyType;

                if (propertyType.IsBHoMType())
                {
                    // Add type to result
                    result.Add(propertyType);

                    // Recurse on sub-properties
                    propertyType.GetProperties().ToList().ForEach(p => AllNestedTypes(p.PropertyType, result));
                }

                if (propertyType.IsGenericType)
                {
                    List<Type> genericArgumentTypes = propertyType.GetGenericArguments().ToList();

                    List<Type> BHoMGenericArgumentTypes = genericArgumentTypes.Where(t => t.IsBHoMType()).ToList();

                    // Add type to result
                    BHoMGenericArgumentTypes.ForEach(bt => result.Add(bt));

                    // Recurse on generic types
                    // TODO: This is limited to generic arguments that are bhom types.
                    // We can expand to also consider generic arguments that are themselves generic types,
                    // e.g. to get also the `Bar` in a type `Dictionary<string, List<Bar>>`.
                    BHoMGenericArgumentTypes.ForEach(bt => AllNestedTypes(bt, result));
                }
            }
        }
    }
}
