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
        public static string TTLDataType_DefaultTypeForUnknownConversion(LocalRepositorySettings r)
        {
            string defaultDataTypeUri = typeof(BH.oM.RDF.JsonSerialized).GithubURI(r)?.ToString();

            // TODO: add better guard against null, possibly adding mechanism to provide a defaultDataType URI rather than a Type.
            defaultDataTypeUri = defaultDataTypeUri ?? "https://github.com/BHoM/RDF_Prototypes/commit/ff8ccb68dbba5aeadb4a9a284f141eb1515e169a";

            string TTLDataType = $"###  {defaultDataTypeUri}\n{typeof(BH.oM.RDF.JsonSerialized).UniqueNodeId()} rdf:type rdfs:Datatype ;";
            TTLDataType += $@"\nrdfs:label ""{typeof(BH.oM.RDF.JsonSerialized).DescriptiveName()}""@en ;";
            TTLDataType += $"\nowl:equivalentClass xsd:string .";

            return TTLDataType;
        }
    }
}
