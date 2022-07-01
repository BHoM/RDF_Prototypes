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
        private static List<string> TTLObjectProperties(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLObjectProperties = new List<string>();

            for (int i = 0; i < cSharpGraph.ObjectProperties.Count; i++)
            {
                var rel = cSharpGraph.ObjectProperties.ElementAt(i);

                string gitHubUri = rel.PropertyInfo.GithubURI(localRepositorySettings)?.ToString();

                if (gitHubUri.IsNullOrEmpty())
                {
                    Log.RecordWarning($"Could not add the {nameof(CSharpGraph)}.{nameof(CSharpGraph.ObjectProperties)}: could not compute its URI.");
                    continue;
                }

                try
                {
                    string TTLObjectProperty = "";

                    if (rel.RangeClass == null || rel.PropertyInfo == null)
                        continue;

                    string propertyURI = rel.PropertyInfo.GithubURI(localRepositorySettings).ToString();
                    TTLObjectProperty += $"\n### {propertyURI}";
                    TTLObjectProperty += $"\n:{rel.PropertyInfo.UniqueNodeId()} rdf:type owl:ObjectProperty ;";
                    TTLObjectProperty += $"\nrdfs:domain :{rel.DomainClass.UniqueNodeId()} ;";
                    TTLObjectProperty += $"\nrdfs:range :{rel.RangeClass.UniqueNodeId()} ;";
                    TTLObjectProperty += "\n" + $@"rdfs:label ""{rel.PropertyInfo.DescriptiveName()}""@en .";

                    TTLObjectProperties.Add(TTLObjectProperty);
                }
                catch (Exception e)
                {
                    Log.RecordError($"Could not add the {nameof(CSharpGraph)}.{nameof(CSharpGraph.ObjectProperties)} at position {i}. Error:\n\t{e.ToString()}");
                }
            }

            return TTLObjectProperties;
        }
    }
}
