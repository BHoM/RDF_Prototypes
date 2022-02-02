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
        public static Type[] TryGetAssemblyTypes(this Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine($"Could not get types for assembly {a.GetName().Name}. Exception:\n {string.Join("\n ", e.LoaderExceptions.Select(le => le.Message).Distinct())}");
            }

            return new Type[] { };
        }
    }
}
