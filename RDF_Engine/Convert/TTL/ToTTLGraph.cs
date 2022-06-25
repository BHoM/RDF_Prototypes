
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts a Graph ontological representation of (BHoM) types and their relations into a TTL format.")]
        public static string ToTTLGraph(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();

            string TTL = Create.TTLHeader(cSharpGraph.OntologySettings.OntologyTitle, cSharpGraph.OntologySettings.OntologyDescription, cSharpGraph.OntologySettings.OntologyBaseAddress);

            TTL += "Annotation Properties".TTLSectionTitle();
            TTL += string.Join("\n", Create.TTLAnnotationProperties());

            TTL += "Datatypes".TTLSectionTitle();
            TTL += string.Join("\n", cSharpGraph.TTLDataTypes(localRepositorySettings));

            TTL += "Classes".TTLSectionTitle();
            TTL += string.Join("\n\n", cSharpGraph.TTLClasses(localRepositorySettings));

            TTL += "Object Properties".TTLSectionTitle();
            TTL += string.Join("\n\n", cSharpGraph.TTLObjectProperties(localRepositorySettings));

            TTL += "Data properties".TTLSectionTitle();
            TTL += string.Join("\n\n", cSharpGraph.TTLDataProperties(localRepositorySettings));

            if (cSharpGraph.AllIndividuals?.Any() ?? false)
            {
                TTL += "Individuals".TTLSectionTitle();
                TTL += string.Join("\n\n", cSharpGraph.TTLIndividuals(localRepositorySettings));
            }

            return TTL;
        }
    }
}
