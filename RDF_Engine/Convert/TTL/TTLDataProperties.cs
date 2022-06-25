﻿using BH.oM.RDF;
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
        public static List<string> TTLDataProperties(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLDataProperties = new List<string>();

            for (int i = 0; i < cSharpGraph.DataProperties.Count; i++)
            {
                var rel = cSharpGraph.DataProperties.ElementAt(i);

                string gitHubUri = rel.PropertyInfo.GithubURI(localRepositorySettings)?.ToString();

                if (gitHubUri.IsNullOrEmpty())
                {
                    Log.RecordWarning($"Could not add the DataProperty relation `{rel.PropertyInfo.Name}` of type `{rel.PropertyInfo.DeclaringType}`: could not compute its URI.");
                    continue;
                }

                try
                {
                    string TTLDataProperty = "";

                    string propertyURI = rel.PropertyInfo.GithubURI(localRepositorySettings).ToString();
                    TTLDataProperty += $"\n### {propertyURI}";
                    TTLDataProperty += $"\n:{rel.PropertyInfo.UniqueNodeId()} rdf:type owl:DatatypeProperty ;";
                    TTLDataProperty += $"\nrdfs:domain :{rel.DomainClass.UniqueNodeId()} ;";

                    // We need to map the Range Type to a valid DataType.
                    string dataType = rel.RangeType.ToOntologyDataType();

                    if (dataType == typeof(Base64JsonSerialized).UniqueNodeId())
                        TTLDataProperty += $"\nrdfs:range :{dataType} ;";
                    else
                        TTLDataProperty += $"\nrdfs:range {dataType} ;";

                    TTLDataProperty += "\n" + $@"rdfs:label ""{rel.PropertyInfo.DescriptiveName()}""@en .";

                    TTLDataProperties.Add(TTLDataProperty);
                }
                catch (Exception e)
                {
                    Log.RecordError($"Could not add the {nameof(CSharpGraph)}.{nameof(CSharpGraph.DataProperties)} at position {i}. Error:\n\t{e.ToString()}");
                }
            }

            // Add the IObject's DataProperty for the Default Data Type
            Type defaultTypeForUnknownConversions = typeof(Base64JsonSerialized);
            string defaultDataType_IObjectProperty = "\n" + $@"###  {defaultTypeForUnknownConversions.GithubURI(localRepositorySettings)}
                {defaultTypeForUnknownConversions.DescriptiveName()} rdf:type owl:DatatypeProperty ;
                rdfs:domain :IObject ;
                rdfs:range {defaultTypeForUnknownConversions.UniqueNodeId()} .";

            return TTLDataProperties;
        }
    }
}