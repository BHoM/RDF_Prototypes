using BH.Engine.Base;
using BH.oM.RDF;
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
        [Description("Extracts a Dictionary representation of a Graph established by the input types and their code relationships." +
            "The Key of the dictionary is the Type, while the Value is the list of edges (relationships).")]
        public static Dictionary<Type, List<IRelation>> DictionaryGraphFromTypes(this IEnumerable<Type> oMTypes, Dictionary<Type, List<IRelation>> existingDictionaryGraph = null)
        {
            Dictionary<Type, List<IRelation>> dictionaryGraph = new Dictionary<Type, List<IRelation>>();

            if (existingDictionaryGraph != null)
                dictionaryGraph = existingDictionaryGraph.DeepClone();

            foreach (Type oMType in oMTypes)
            {
                // Check if this Type has been already added to the Graph, otherwise initialise the edges list.
                List<IRelation> edges = null;
                if (!dictionaryGraph.TryGetValue(oMType, out edges))
                    edges = new List<IRelation>();

                // Parse the existing code-relationships.
                edges.AddRange(oMType.RelationsFromType());

                dictionaryGraph[oMType] = edges;
            }

            return dictionaryGraph;
        }

        public static Dictionary<Type, List<IRelation>> DictionaryGraphFromTypes(string typesAssembliesFolder = @"C:\ProgramData\BHoM\Assemblies")
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(typesAssembliesFolder, true);

            // Remove duplicate classes in the same file, e.g. `BH.oM.Base.Output` which has many generics replicas.
            List<TypeInfo> oMTypes = new List<TypeInfo>(oMassemblies.SelectMany(a => a.DefinedTypes).GroupBy(t => t.FullName.OnlyAlphabeticAndDots()).Select(g => g.First()));

            // Take a subset of the types avaialble to reduce the size of the output graph. This can become a Filter function.
            IEnumerable<TypeInfo> onlyBaseOmTypes = oMTypes.Where(t => t != null && t.Namespace != null && t.Namespace.EndsWith("BH.oM.Base")).ToList();
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name == "NamedNumericTolerance" || t.Name == "IObject");
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("Output"));

            // Extract a dictionary representation of the BHoM Ontology Graph
            Dictionary<Type, List<IRelation>> dictionaryGraph = onlyBaseOmTypes.DictionaryGraphFromTypes();

            return dictionaryGraph;
        }
    }
}
