using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static void WriteWebVOWLOntologiesPerNamespace(LocalRepositorySettings settings)
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            List<TypeInfo> oMTypes = oMassemblies.BHoMTypes();
            string saveDirRelativeToRepoRoot = settings.SaveDir_RelativeToRoot;

            // Take a subset of the types avaialble to reduce the size of the output graph. This can become a Filter function.
            //IEnumerable<TypeInfo> onlyBaseOmTypes = oMTypes.Where(t => t != null && t.Namespace != null && t.Namespace.EndsWith("BH.oM.Base")).ToList();
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name == "NamedNumericTolerance" || t.Name == "IObject");
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("Output"));
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("ComparisonConfig"));

            SortedDictionary<string, string> webVOWLJsonsPerNamespace = Engine.RDF.Compute.WebVOWLJsonPerNamespace(oMTypes, settings);

            // Save all generated ontologies to file
            foreach (var kv in webVOWLJsonsPerNamespace)
                kv.Value.WriteToJsonFile($"{kv.Key}.json", $"..\\..\\..\\{saveDirRelativeToRepoRoot}");

            // Save the URLS to the ontologies. These are links to the WebVOWL website with a parameter passed that links directly the Github URL of the ontology.
            string allWebOWLOntologyURL = $"..\\..\\..\\{saveDirRelativeToRepoRoot}\\_allWebOWLOntologyURL.txt";

            File.WriteAllText(allWebOWLOntologyURL, ""); // empty the file
            foreach (var kv in webVOWLJsonsPerNamespace)
            {
                string WebVOWLOntologyURL = $"https://service.tib.eu/webvowl/#url=https://raw.githubusercontent.com/BHoM/RDF_Prototypes/main/{saveDirRelativeToRepoRoot}/{kv.Key}.json";
                File.AppendAllText(allWebOWLOntologyURL, "\n" + WebVOWLOntologyURL);
            }
        }
    }
}
