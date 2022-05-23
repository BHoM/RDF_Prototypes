using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
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
        [Description("Returns the type of the input object(s).")]
        public static List<List<Type>> GetType(this List<List<object>> obj)
        {
            return obj.Select(l => l.Select(o => o.GetType()).ToList()).ToList();
        }
    }
}
