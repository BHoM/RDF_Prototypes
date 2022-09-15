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
                ObjectProperty objectProperty = cSharpGraph.ObjectProperties.ElementAt(i) as ObjectProperty;

                string gitHubUri = objectProperty.PropertyInfo.OntologyURI(cSharpGraph.OntologySettings.TBoxSettings, localRepositorySettings)?.ToString();

                if (gitHubUri.IsNullOrEmpty())
                {
                    Log.RecordWarning($"Could not add the {nameof(CSharpGraph)}.{nameof(CSharpGraph.ObjectProperties)}: could not compute its URI.");
                    continue;
                }

                try
                {
                    string TTLObjectProperty = "";

                    if (objectProperty.RangeClass == null || objectProperty.PropertyInfo == null)
                        continue;

                    string propertyURI = gitHubUri;
                    TTLObjectProperty += $"\n### {propertyURI}";
                    TTLObjectProperty += $"\n:{objectProperty.PropertyInfo.UniqueNodeId()} rdf:type owl:ObjectProperty";
                    if (objectProperty is OWLObjectProperty owlobjectprop)
                        if (owlobjectprop.OWLObjectPropertyType != OWLObjectPropertyType.Undefined) 
                            TTLObjectProperty += $", owl:{owlobjectprop.OWLObjectPropertyType}Property";
                        
                    TTLObjectProperty += ";";

                    TTLObjectProperty += $"\nrdfs:domain :{objectProperty.DomainClass.UniqueNodeId()};";
                    TTLObjectProperty += $"\nrdfs:range :{objectProperty.RangeClass.UniqueNodeId()};";
                    TTLObjectProperty += "\n" + $@"rdfs:label ""{objectProperty.PropertyInfo.DescriptiveName()}""@en.";

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
