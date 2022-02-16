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
        [Description("Returns the Full Name of the input Type, only including characters that are alphanumeric, dots and/or greek letters (which are useful for BH.oM.Structure objects properties).")]
        public static string FullNameValidChars(this Type type)
        {
            return RemoveInvalidChars(type.FullName ?? $"{type.Namespace}.{type.Name}");
        }

        /***************************************************/

        [Description("Returns the Name of the input Type, only including characters that are alphanumeric, dots and/or greek letters (which are useful for BH.oM.Structure objects properties).")]
        public static string NameValidChars(this Type type)
        {
            return RemoveInvalidChars(type.Name);
        }

        /***************************************************/

        private static string RemoveInvalidChars(string text)
        {
            if (text.Contains("`"))
            {
                // remove those weird chars that sometimes happen e.g. IElementLoad`1
                text = text.Substring(0, text.IndexOf("`"));

                while (Char.IsDigit(text.Last()))
                    text = text.Substring(0, text.Length - 1); // if last char is a number, remove it.
            }

            Regex rgx = new Regex(@"[^a-zA-Z0-9.\p{IsGreek}]"); //only alphanumeric, dots and greek letters (useful for structural props).
            text = rgx.Replace(text, "");

            return text;
        }
    }
}