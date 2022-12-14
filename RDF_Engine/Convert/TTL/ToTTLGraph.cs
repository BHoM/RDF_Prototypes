
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        public static string ToTTLGraph(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings = null, string filepath = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();

            StreamWriter sw = null;
            if (!string.IsNullOrWhiteSpace(filepath))
                sw = new StreamWriter(filepath);

            StringBuilder TTL = null;

            try
            {
                TTL = new StringBuilder();

                TTL.Append(Create.TTLHeader(cSharpGraph.OntologySettings, cSharpGraph.OntologySettings.OntologyTitle, cSharpGraph.OntologySettings.OntologyDescription, cSharpGraph.OntologySettings.OntologyBaseAddress));

                TTL.Append("Annotation Properties".TTLSectionTitle());
                TTL.Append(string.Join("\n", Create.TTLAnnotationProperties()));

                TTL.Append("Datatypes".TTLSectionTitle());
                TTL.Append(string.Join("\n", cSharpGraph.TTLDataTypes(localRepositorySettings)));

                TTL.Append("Classes".TTLSectionTitle());

                // Write TBOX settings
                TBoxSettings defaultTboxSettings = new TBoxSettings();
                StringBuilder tBoxSettingsStringBuilder = new StringBuilder();
                if (cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes != defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)
                    tBoxSettingsStringBuilder.Append($"\n# {nameof(defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)}: " + cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes);

                if (cSharpGraph.OntologySettings.TBoxSettings.DefaultBaseUriForUnknownTypes != defaultTboxSettings.DefaultBaseUriForUnknownTypes)
                    tBoxSettingsStringBuilder.Append($"\n# {nameof(defaultTboxSettings.DefaultBaseUriForUnknownTypes)}: " + cSharpGraph.OntologySettings.TBoxSettings.DefaultBaseUriForUnknownTypes);

                if (cSharpGraph.OntologySettings.TBoxSettings.CustomObjectTypesBaseAddress != defaultTboxSettings.CustomObjectTypesBaseAddress)
                    tBoxSettingsStringBuilder.Append($"\n# {nameof(defaultTboxSettings.CustomObjectTypesBaseAddress)}: " + cSharpGraph.OntologySettings.TBoxSettings.CustomObjectTypesBaseAddress);

                if (cSharpGraph.OntologySettings.TBoxSettings.CustomobjectsTypeKey != defaultTboxSettings.CustomobjectsTypeKey)
                    tBoxSettingsStringBuilder.Append($"\n# {nameof(defaultTboxSettings.CustomobjectsTypeKey)}: " + cSharpGraph.OntologySettings.TBoxSettings.CustomobjectsTypeKey);

                if (cSharpGraph.OntologySettings.TBoxSettings.TypeUris?.Any() ?? false)
                {
                    string typeUriString = $@"{string.Join($"\n# {nameof(defaultTboxSettings.TypeUris)}: ", cSharpGraph.OntologySettings.TBoxSettings.TypeUris.Select(KV => KV.Key.AssemblyQualifiedName + "; " + KV.Value.ToString() ))}";
                    tBoxSettingsStringBuilder.Append($"\n# {nameof(defaultTboxSettings.TypeUris)}: " + typeUriString);
                }

                string tBoxSettingsString = tBoxSettingsStringBuilder.ToString();
                if (tBoxSettingsString.Any())
                {
                    tBoxSettingsString = $"# {nameof(TBoxSettings)}" + tBoxSettingsString + $"\n# {nameof(TBoxSettings)}\n\n";
                    TTL.Append(tBoxSettingsString);
                }

                TTL.Append(string.Join("\n\n", cSharpGraph.TTLClasses(localRepositorySettings)));

                TTL.Append("Object Properties".TTLSectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLObjectProperties(localRepositorySettings)));

                TTL.Append("Data properties".TTLSectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLDataProperties(localRepositorySettings)));

                // Write ABOX settings

                ABoxSettings defaultAboxSettings = new ABoxSettings();
                StringBuilder aBoxSettingsStringBuilder = new StringBuilder();

                if (cSharpGraph.AllIndividuals?.Any() ?? false)
                {
                    TTL.Append("Individuals".TTLSectionTitle());

                    if (cSharpGraph.OntologySettings.ABoxSettings.IndividualsBaseAddress != defaultAboxSettings.IndividualsBaseAddress)
                        aBoxSettingsStringBuilder.Append($"\n# {nameof(defaultAboxSettings.IndividualsBaseAddress)}: " + cSharpGraph.OntologySettings.ABoxSettings.IndividualsBaseAddress);

                    if (cSharpGraph.OntologySettings.ABoxSettings.ConsiderDefaultPropertyValues != defaultAboxSettings.ConsiderDefaultPropertyValues)
                        aBoxSettingsStringBuilder.Append($"\n# {nameof(defaultAboxSettings.ConsiderDefaultPropertyValues)}: " + cSharpGraph.OntologySettings.ABoxSettings.ConsiderDefaultPropertyValues);

                    if (cSharpGraph.OntologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues != defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)
                        aBoxSettingsStringBuilder.Append($"\n# {nameof(defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)}: " + cSharpGraph.OntologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues);

                    string aBoxSettingsString = aBoxSettingsStringBuilder.ToString();
                    if (aBoxSettingsString.Any())
                    {
                        aBoxSettingsString = $"# {nameof(ABoxSettings)}" + aBoxSettingsString + $"\n# {nameof(ABoxSettings)}\n\n";
                        TTL.Append(aBoxSettingsString);
                    }

                    cSharpGraph.TTLIndividuals(localRepositorySettings, TTL);
                }
            }

            catch { }

            if (sw != null)
            {
                sw.Write(TTL);
                sw.Close();
            }

            return TTL.ToString();
        }
    }
}
