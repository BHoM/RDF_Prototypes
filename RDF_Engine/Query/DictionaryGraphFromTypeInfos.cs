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
    }
}
