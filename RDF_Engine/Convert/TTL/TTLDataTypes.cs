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
    public static partial class Convert
    {
        private static List<string> TTLDataTypes(this CSharpGraph cSharpGraph, LocalRepositorySettings r)
        {
            List<string> dataTypes = new List<string>();

            dataTypes.Add(DefaultDataTypeForUnknownConversion(cSharpGraph.OntologySettings.TBoxSettings, r));

            return dataTypes;
        }

        private static string DefaultDataTypeForUnknownConversion(TBoxSettings tboxSettings, LocalRepositorySettings r)
        {
            string defaultDataTypeUri = typeof(BH.oM.RDF.Base64JsonSerialized).OntologyUri(tboxSettings, r)?.ToString();

            // TODO: add better guard against null, possibly adding mechanism to provide a defaultDataType URI rather than a Type.
            defaultDataTypeUri = defaultDataTypeUri ?? "https://github.com/BHoM/RDF_Prototypes/commit/ff8ccb68dbba5aeadb4a9a284f141eb1515e169a";
            
            string TTLDataType = $"### {defaultDataTypeUri}\n:{typeof(BH.oM.RDF.Base64JsonSerialized).UniqueNodeId()} rdf:type rdfs:Datatype ;";
            TTLDataType += "\n"+$@"rdfs:label ""{typeof(BH.oM.RDF.Base64JsonSerialized).DescriptiveName()}""@en .";

            return TTLDataType;
        }
    }
}
