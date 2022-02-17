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
using Newtonsoft.Json.Linq;
using BH.Engine.Base;
using log = BH.oM.RDF.Log;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args = null)
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            List<TypeInfo> oMTypes = oMassemblies.BHoMTypes();

            // Take a subset of the types avaialble to reduce the size of the output graph. This can become a Filter function.
            //IEnumerable<TypeInfo> onlyBaseOmTypes = oMTypes.Where(t => t != null && t.Namespace != null && t.Namespace.EndsWith("BH.oM.Base")).ToList();
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name == "NamedNumericTolerance" || t.Name == "IObject");
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("Output"));
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("ComparisonConfig"));

            SortedDictionary<string, string> webVOWLJsonsPerNamespace = Engine.RDF.Compute.WebVOWLJsonPerNamespace(oMTypes);
            string generatedOntologiesDirectoryName = "WebVOWLOntology";

            // Save all generated ontologies to file
            foreach (var kv in webVOWLJsonsPerNamespace)
                kv.Value.WriteToJsonFile($"{kv.Key}.json", $"..\\..\\..\\{generatedOntologiesDirectoryName}");

            // Save the URLS to the ontologies. These are links to the WebVOWL website with a parameter passed that links directly the Github URL of the ontology.
            string allWebOWLOntologyURL = $"..\\..\\..\\{generatedOntologiesDirectoryName}\\_allWebOWLOntologyURL.txt";

            File.WriteAllText(allWebOWLOntologyURL, ""); // empty the file
            foreach (var kv in webVOWLJsonsPerNamespace)
            {
                string WebVOWLOntologyURL = $"https://service.tib.eu/webvowl/#url=https://raw.githubusercontent.com/BHoM/RDF_Prototypes/main/{generatedOntologiesDirectoryName}/{kv.Key}.json";
                File.AppendAllText(allWebOWLOntologyURL, "\n" + WebVOWLOntologyURL);
            }

            //List<Type> 
            //Dictionary<Type, List<IRelation>> dictionaryGraph = group.DictionaryGraphFromTypes();
            //string webVOWLJson = Engine.RDF.Convert.ToWebVOWLJson(dictionaryGraph);

            //result[group.Key] = webVOWLJson;


            // Invoke all static methods in `Tests_Alessio` class
            //typeof(Tests_Alessio).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            // Invoke all static methods in `Tests_Diellza` class
            //typeof(Tests_Diellza).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
            log.SaveLogToDisk("..\\..\\..\\log.txt");
        }
    }
}
