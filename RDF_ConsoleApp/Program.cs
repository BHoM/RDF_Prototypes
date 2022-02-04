using BH.oM.Analytical.Results;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.RDF;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.oM.Architecture.Elements;
using BH.oM.Physical.Elements;
using BH.oM.RDF;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    class Program
    {
        public static void Main(string[] args = null)
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);

            HashSet<TypeInfo> oMTypes = new HashSet<TypeInfo>(oMassemblies.SelectMany(a => a.DefinedTypes));

            // Extract a dictionary representation of the BHoM Ontology Graph
            IEnumerable<TypeInfo> onlyBaseOmTypes = oMTypes.Where(t => t != null && t.Namespace != null && t.Namespace.EndsWith("BH.oM.Base")).ToList();
            Dictionary<Type, List<IRelation>> dictionaryGraph = onlyBaseOmTypes.DictionaryGraphFromTypeInfos();


            // Invoke all static methods in `Tests_Alessio` class
            typeof(Tests_Alessio).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            // Invoke all static methods in `Tests_Diellza` class
            typeof(Tests_Diellza).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));
        }
    }
}
