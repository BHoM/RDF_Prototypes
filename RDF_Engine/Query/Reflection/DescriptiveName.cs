using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Returns a Member's type name in a readable, descriptive format. E.g. for a Property called 'Elements' that is a list of strings, returns `Elements (List<string>)`." +
            "If the Member is a Property, returns the name of the property. If the parent type is an enum, returns nothing." +
            "In all other cases, returns the member reflectedtype name.")]
        public static string DescriptiveName(this MemberInfo mi)
        {
            if (mi == null)
                return null;

            if (mi.DeclaringType?.IsEnum ?? false)
                return ""; // if the parent type is an enum, return nothing.

            try
            {
                PropertyInfo pi = mi as PropertyInfo;
                if (pi != null)
                    return pi.DescriptiveName();
            }
            catch { }

            return mi.ReflectedType?.DescriptiveName() ?? "";
        }


        /***************************************************/

        public static string DescriptiveName(this PropertyInfo pi, bool includeFullPath = true)
        {
            // Custom Type exception.
            if (pi is CustomPropertyInfo)
                return pi.Name;

            return includeFullPath ? pi.Name + $" ({pi.DeclaringType.FullNameValidChars()}.{pi.Name})" : pi.Name;
        }

        /***************************************************/

        [Description("Returns a Type's name in a readable, descriptive format. E.g. for a list of strings, return List<string>.")]
        public static string DescriptiveName(this Type t, bool includeNamespace = false)
        {
            // Custom Type exception.
            if (t is CustomType)
                return t.Name;

            string descriptiveName = "";

            if (!t.IsGenericType)
                descriptiveName = t.NameValidChars();
            else
            {
                string genericTypeName = t.GetGenericTypeDefinition().NameValidChars();

                string genericArgs = string.Join(",",
                    t.GetGenericArguments()
                        .Select(ta => DescriptiveName(ta)).ToArray());

                descriptiveName = genericTypeName + "<" + genericArgs + ">";
            }

            if (includeNamespace)
                descriptiveName += $" ({t.Namespace})";

            return descriptiveName;
        }

        /***************************************************/

        public static string DescriptiveName(this object obj)
        {
            return obj.GetType().DescriptiveName();
        }
    }
}