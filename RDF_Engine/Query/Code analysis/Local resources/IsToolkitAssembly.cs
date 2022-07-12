using System;
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
        private static Dictionary<Assembly, string> m_CachedToolkitNames = new Dictionary<Assembly, string>();

        public static bool IsToolkitAssembly(this Assembly assembly, out string toolkitName, string dllDirectory = @"C:\ProgramData\BHoM\Assemblies")
        {
            toolkitName = null;

            if (m_CachedToolkitNames.TryGetValue(assembly, out toolkitName))
                return true;

            var assemblies = Compute.LoadAssembliesInDirectory(dllDirectory, true, false);
            var toolkitAssemblies = RDF.Query.ToolkitAssemblies(assemblies);

            foreach (var kv in toolkitAssemblies)
            {
                if (kv.Value.Contains(assembly))
                {
                    toolkitName = kv.Key;
                    m_CachedToolkitNames[assembly] = toolkitName;
                    return true;
                }
            }

            return false;
        }
    }
}
