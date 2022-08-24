
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.RDF;
using BH.oM.Base.Attributes;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Computes a TTL ontology with the input IObjects. The ontology will include both T-Box and A-Box." +
             "The T-Box is constructed from the Types of the input objects, and their relations, expressed via the CSharp object properties.")]
        public static void TTLGraph(this List<object> objects, string filePath, OntologySettings ontologySettings, LocalRepositorySettings localRepositorySettings = null)
        {
            if (localRepositorySettings == null)
                localRepositorySettings = new LocalRepositorySettings();

            CSharpGraph cSharpGraph = Engine.RDF.Compute.CSharpGraph(objects, ontologySettings);

            cSharpGraph.ToTTLGraph(localRepositorySettings, filePath);
        }


        [Description("Computes a TTL ontology with the input IObjects. The ontology will include both T-Box and A-Box." +
            "The T-Box is constructed from the Types of the input objects, and their relations, expressed via the CSharp object properties.")]
        public static string TTLGraph(this List<object> objects, OntologySettings ontologySettings, LocalRepositorySettings localRepositorySettings = null)
        {
            if (localRepositorySettings == null)
                localRepositorySettings = new LocalRepositorySettings();

            CSharpGraph cSharpGraph = Engine.RDF.Compute.CSharpGraph(objects, ontologySettings);

            string TTL = cSharpGraph.ToTTLGraph(localRepositorySettings);

            return TTL;
        }

        /***************************************************/

        [Description("Computes a TTL T-Box ontology with the input Types." +
            "To compute an ontology that includes both T-Box and A-Box, use the TTLGraph method that takes a list of IObjects, and provide input objects (instances) instead of Types.")]
        public static string TTLGraph(this List<Type> types, OntologySettings ontologySettings, LocalRepositorySettings localRepositorySettings = null)
        {
            if (localRepositorySettings == null)
                localRepositorySettings = new LocalRepositorySettings();

            CSharpGraph cSharpGraph = Engine.RDF.Compute.CSharpGraph(types, ontologySettings);

            string TTL = cSharpGraph.ToTTLGraph(localRepositorySettings);

            return TTL;
        }
    }
}
