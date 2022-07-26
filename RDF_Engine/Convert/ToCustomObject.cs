using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        [Description("Attempts to get the properties of the object and use them to populate a BHoM CustomObject.")]
        public static CustomObject ToCustomObject(this object obj)
        {
            var customObject = new CustomObject();
            bool successfulConversion = false;
            // For Speckle custom objects, we can get the hidden dynamic members by attempting an invokation of GetMembers.
            var method = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => m.Name.Contains("GetMembers")).First();
            if (method != null)
            {
                // The result should be castable if the original object was a speckle custom object.
                var valuesDict = method.Invoke(obj, null) as Dictionary<string, object>;
                if (valuesDict != null)
                    foreach (var kv in valuesDict)
                        customObject.CustomData[kv.Key] = kv.Value;
                successfulConversion = true;
            }
            if (!successfulConversion)
            {
                // TODO: decide whether to expose the option to get other properties (e.g private)
                bool includePrivateProperties = false;
                var bindingFlags = includePrivateProperties ? BindingFlags.NonPublic | BindingFlags.Public : BindingFlags.Public;
                // Try to convert based on any public property.
                var publicProperties = obj.GetType().GetProperties(bindingFlags);
                if (publicProperties != null)
                    foreach (var prop in publicProperties)
                        customObject.CustomData[prop.Name] = prop.GetValue(obj);
            }
            return customObject;
        }
    }
}