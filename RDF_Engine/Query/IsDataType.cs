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

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static bool IsDataType(this Type type)
        {
            return !type.IsOntologyClass();
        }
    }
}
