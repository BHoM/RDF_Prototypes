using BH.Engine.Base;
using BH.Engine.RDF.Types;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Log = BH.Engine.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Type[] TryGetTypes(this Assembly a, bool logWarning = true)
        {
            try
            {
                return a.GetTypes();
            }
            catch(ReflectionTypeLoadException ex)
            {
                if (logWarning)
                    Log.RecordWarning($"Could not load types from Assembly {a.FullName}. Exception(s):\n{ex.LoaderExceptions.Select(e => e.Message.SplitInLinesAndTabify())}", doNotRepeat: true);
            }
            return new Type[] { };
        }

    }
}
