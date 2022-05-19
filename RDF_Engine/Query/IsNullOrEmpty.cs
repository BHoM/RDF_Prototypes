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
        [Description("Determines whether the input object is null; if the object is an IEnumerable, determines if the IEnumerable is empty.")]
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;

            IEnumerable ienum = obj as IEnumerable;
            if (ienum != null && !ienum.GetEnumerator().MoveNext())
                return true;

            return false;
        }
    }
}