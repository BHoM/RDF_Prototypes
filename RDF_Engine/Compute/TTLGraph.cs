
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
using log = BH.oM.RDF.Log;
using BH.Engine.Base;
using BH.oM.RDF;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static string TTLGraph(this IObject iObject, OntologySettings ontologySettings, LocalRepositorySettings localRepositorySettings)
        {
            CSharpGraph cSharpGraph = Engine.RDF.Compute.CSharpGraph(iObject, new OntologySettings());

            string TTL = cSharpGraph.ToTTLGraph(localRepositorySettings);

            return TTL;
        }

        public static string TTLGraph(this Type type, OntologySettings ontologySettings)
        {
            if (!typeof(IObject).IsAssignableFrom(type))
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the Graph of a non-IObject type.");
                return null;
            }

            throw new NotImplementedException("Not yet implemented");
        }
    }
}
