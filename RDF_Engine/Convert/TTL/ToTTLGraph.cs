
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

                TTL.Append(Create.TTLHeader(cSharpGraph.OntologySettings,cSharpGraph.OntologySettings.OntologyTitle, cSharpGraph.OntologySettings.OntologyDescription, cSharpGraph.OntologySettings.OntologyBaseAddress));

                TTL.Append("Annotation Properties".TTLSectionTitle());
                TTL.Append(string.Join("\n", Create.TTLAnnotationProperties()));

                TTL.Append("Datatypes".TTLSectionTitle());
                TTL.Append(string.Join("\n", cSharpGraph.TTLDataTypes(localRepositorySettings)));

                TTL.Append("Classes".TTLSectionTitle());


                // TypeUris

                /*if (!object.Equals(cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes, defaultSettings.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes))
                {
                    TTL.Append("\n#TreatCustomObjectsWithTypeKeyAsCustomObjectTypes: " + cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes + " #TCOWTK");
                }
                */

                if (cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes != true) // add default property comparison
                {
                    TTL.Append("\n#TreatCustomObjectsWithTypeKeyAsCustomObjectTypes: " + cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes + " #TCOWTK");
                }

                /*if (cSharpGraph.OntologySettings.TBoxSettings.DefaultBaseUriForUnknownTypes == true)
                {
                    TTL.Append("\n\n#TreatCustomObjectsWithTypeKeyAsCustomObjectTypes: " + cSharpGraph.OntologySettings.TBoxSettings.DefaultBaseUriForUnknownTypes + " #DBUFUT");
                }
                */

                /*if (cSharpGraph.OntologySettings.TBoxSettings.CustomobjectsTypeKey == ) // If default
                {
                    TTL.Append("\n\n#CustomobjectsTypeKey: " + cSharpGraph.OntologySettings.TBoxSettings.CustomobjectsTypeKey + " #CTK");
                }
                */
                
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLClasses(localRepositorySettings)));

                TTL.Append("Object Properties".TTLSectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLObjectProperties(localRepositorySettings)));

                TTL.Append("Data properties".TTLSectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLDataProperties(localRepositorySettings)));

                if (cSharpGraph.AllIndividuals?.Any() ?? false)
                {
                    TTL.Append("Individuals".TTLSectionTitle());
                    TTL.Append("\n#IndividualsBaseAdress :URL " + cSharpGraph.OntologySettings.ABoxSettings.IndividualsBaseAddress + " #URL");

                    if (cSharpGraph.OntologySettings.ABoxSettings.ConsiderDefaultPropertyValues != true) // replace with default settings!
                    {
                        TTL.Append("\n#ConsiderDefaultPropertyValues: " + cSharpGraph.OntologySettings.ABoxSettings.ConsiderDefaultPropertyValues + " #CDPV");
                    }
                    if (cSharpGraph.OntologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues != false) // replace with default settings!
                    {
                        TTL.Append("\n#ConsiderNullOrEmptyPropertyValues: " + cSharpGraph.OntologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues + " #CNOEPV");
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
