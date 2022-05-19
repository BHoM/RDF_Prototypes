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
using QuikGraph;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Extracts a Dictionary representation of a Graph established by the input types and their code relationships." +
            "The Key of the dictionary is the Type, while the Value is the list of edges (relationships).")]
        public static Dictionary<TypeInfo, List<IRelation>> GraphFromTypeInfos(this IEnumerable<TypeInfo> oMTypes)
        {
            Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = new Dictionary<TypeInfo, List<IRelation>>();

            // Iterate TypeInfos and generate all edges (relationships) recursively.

            foreach (TypeInfo oMType in oMTypes)
            {
                // Check if this Type has been already added to the Graph, otherwise initialise the edges list.
                List<IRelation> edges = null;
                if (!dictionaryGraph.TryGetValue(oMType, out edges))
                    edges = new List<IRelation>();

                // Parse the existing code-relationships.
                var relationsFromType = oMType.RelationsFromType();
                edges.AddRange(relationsFromType);

                dictionaryGraph[oMType] = edges;
            }

            return dictionaryGraph;
        }

        private static void EdgesFromType(this TypeInfo oMType, object instance = null, int recursionLevel = 0, int maxRecursion = -1)
        {
            List<IRelation> resultRelations = new List<IRelation>();

            PropertyInfo[] properties = null;
            properties = oMType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            IRelation propertyRelation = null;
            foreach (PropertyInfo property in properties)
            {
                if (oMType.IsInterface)
                    propertyRelation = new RequiresProperty() { Subject = oMType, Object = property };
                else
                    propertyRelation = new HasProperty() { Subject = oMType, Object = property };

                resultRelations.Add(propertyRelation);
            }

            Type[] implementedInterfaces = oMType.GetInterfaces();
            foreach (Type implementedInterface in implementedInterfaces)
            {
                propertyRelation = new IsA() { Subject = oMType, Object = implementedInterface };

                resultRelations.Add(propertyRelation);
            }

            Type baseType = oMType.BaseType;
            if (baseType != null)
                resultRelations.Add(new IsSubclassOf() { Subject = oMType, Object = baseType });
        }
    }
}
