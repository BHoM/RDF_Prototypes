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
        [Description("Get all the assemblies belonging to all Toolkits.")]
        public static Dictionary<string, List<Assembly>> ToolkitAssemblies(this List<Assembly> assemblies, bool includeOm = true, bool includeEngine = true, bool includeAdapter = true)
        {
            // The easy way is not reliable. We dont write the name consistently.
            //var gna = assemblies
            //    .Where(a => a.GetCustomAttribute<AssemblyProductAttribute>().Product.EndsWith("_Toolkit")); 

            // Try get from cache.
            Dictionary<string, List<Assembly>> result = new Dictionary<string, List<Assembly>>();
            if (m_cachedToolkitAssemblies.TryGetValue(assemblies, out result))
                return result;

            HashSet<string> allAdapterNames = new HashSet<string>(assemblies
                .Where(a => a.GetName().Name.ToLower().Contains("adapter") && a.GetName().Name != "BHoM_Adapter")
                .Select(a => a.GetName().Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault())
                .Where(n => n != "Adapter")
                );

            // Collect all assemblies that have an AssemblyProduct name that ends with _Toolkit(e.g. "IES_Toolkit")
            Dictionary<string, List<Assembly>> allToolkitAssemblies = assemblies
               .Where(a =>
               {
                   string projName = a.GetCustomAttribute<AssemblyTitleAttribute>().Title;

                   //if (a.GetName().Name.Contains("Revit"))
                   //    a = a;

                   //string assemblyTitle = a.GetCustomAttribute<AssemblyTitleAttribute>().Title;
                   //string assemblyName = a.GetName().Name;

                   return (includeOm && projName.Contains("_oM")) || (includeEngine && projName.Contains("_Engine")) || (includeAdapter && projName.Contains("_Adapter"));
               })
               .Where(a => allAdapterNames.Contains(a.GetName().Name.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()))
               .GroupBy(a => a.GetCustomAttribute<AssemblyTitleAttribute>().Title.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() + "_Toolkit")
               .ToDictionary(g => g.Key, g => g.ToList());

            m_cachedToolkitAssemblies[assemblies] = allToolkitAssemblies;

            return allToolkitAssemblies;
        }

        private static Dictionary<List<Assembly>, Dictionary<string, List<Assembly>>> m_cachedToolkitAssemblies = new Dictionary<List<Assembly>, Dictionary<string, List<Assembly>>>();
    }
}
