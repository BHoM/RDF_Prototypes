using System;
using System.CodeDom;
using System.CodeDom.Compiler;
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
        public static List<Type> GetInterfacesNoRedundancy(this Type sourceType)
        {
            return BaseTypesNoRedundancy(sourceType).Where(t => t.IsInterface).ToList();
        }
    }
}
