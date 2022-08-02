using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Reflection;
using System.Collections;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        [Description("Attempts to get the properties of the object and use them to populate a BHoM CustomObject.")]
        public static object ToBHoM(this object obj)
        {
            if (obj == null)
                return null;

            Type objType = obj.GetType();

            if (obj is IObject || objType.IsPrimitive() || objType.IsIEnumOfPrimitives())
                return obj;

            var customObject = new CustomObject();

            // For Speckle custom objects, we can get the hidden dynamic members by attempting an invokation of GetMembers.
            var method = objType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.Name.Contains("GetMembers")).FirstOrDefault();
            if (method != null)
            {
                // The result should be castable if the original object was a speckle custom object.
                var valuesDict = method.Invoke(obj, null) as Dictionary<string, object>;
                if (valuesDict != null)
                    foreach (var kv in valuesDict)
                        customObject.CustomData[Query.RemoveInvalidChars(kv.Key)] = kv.Value.ToBHoM();

                return customObject;
            }


            // TODO: decide whether to expose the option to get other properties (e.g private)
            bool includePrivateProperties = false;
            var bindingFlags = includePrivateProperties ? BindingFlags.NonPublic | BindingFlags.Public : BindingFlags.Public;
            // Try to convert based on any public property.
            var publicProperties = objType.GetProperties(bindingFlags);
            if (publicProperties != null)
                foreach (var prop in publicProperties)
                    customObject.CustomData[prop.NameValidChars()] = prop.GetValue(obj).ToBHoM();

            return customObject;
        }

        public static bool IsIEnumOfPrimitives(this Type t)
        {
            if (t is IEnumerable)
            {
                Type[] genericArgs = t.GetGenericArguments();

                if (genericArgs.Length == 1 && (genericArgs.FirstOrDefault()?.IsPrimitive() ?? false))
                    return true;
            }

            return false;
        }
    }
}