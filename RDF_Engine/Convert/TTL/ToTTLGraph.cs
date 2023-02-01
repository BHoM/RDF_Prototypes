/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */


using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.RDF;
using Microsoft.Extensions.Primitives;
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

                TTL.Append(Create.TTLHeader(cSharpGraph.OntologySettings));

                TTL.Append("Annotation Properties".TTLSectionTitle());
                TTL.Append(string.Join("\n", Create.TTLAnnotationProperties()));

                TTL.Append("Datatypes".TTLSectionTitle());
                TTL.Append(string.Join("\n", cSharpGraph.TTLDataTypes(localRepositorySettings)));

                TTL.Append("Classes".TTLSectionTitle());

                AddTBoxSettings(cSharpGraph, TTL);

                TTL.Append(string.Join("\n\n", cSharpGraph.TTLClasses(localRepositorySettings)));

                TTL.Append("Object Properties".TTLSectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLObjectProperties(localRepositorySettings)));

                TTL.Append("Data properties".TTLSectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLDataProperties(localRepositorySettings)));

                AddABoxSettings(cSharpGraph, localRepositorySettings, TTL);

                cSharpGraph.TTLIndividuals(localRepositorySettings, TTL);

                TTL.Append("Footer".TTLSectionTitle());
                TTL.AppendLine($"# {nameof(OntologySettings)}: {cSharpGraph.OntologySettings.ToBase64JsonSerialized()}");
            }

            catch { }

            if (sw != null)
            {
                sw.Write(TTL);
                sw.Close();
            }

            return TTL.ToString();
        }

        // Adds TBox Settings in a human-readable way. The settings are however serialized-deserialized from a specific comment in the header.
        private static void AddTBoxSettings(CSharpGraph cSharpGraph, StringBuilder TTL)
        {
            TBoxSettings defaultTboxSettings = new TBoxSettings();
            StringBuilder tBoxSettingsStringBuilder = new StringBuilder();
            if (cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes != defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)}: " + cSharpGraph.OntologySettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes);

            if (cSharpGraph.OntologySettings.TBoxSettings.DefaultBaseUriForUnknownTypes != defaultTboxSettings.DefaultBaseUriForUnknownTypes)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.DefaultBaseUriForUnknownTypes)}: " + cSharpGraph.OntologySettings.TBoxSettings.DefaultBaseUriForUnknownTypes);

            if (cSharpGraph.OntologySettings.TBoxSettings.CustomobjectsTypeKey != defaultTboxSettings.CustomobjectsTypeKey)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.CustomobjectsTypeKey)}: " + cSharpGraph.OntologySettings.TBoxSettings.CustomobjectsTypeKey);

            if (cSharpGraph.OntologySettings.TBoxSettings.CustomObjectTypesBaseAddress != defaultTboxSettings.CustomObjectTypesBaseAddress)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.CustomObjectTypesBaseAddress)}: " + cSharpGraph.OntologySettings.TBoxSettings.CustomObjectTypesBaseAddress);

            if (cSharpGraph.OntologySettings.TBoxSettings.TypeUris?.Any() ?? false)
            {
                string typeUriString = $@"{string.Join($"\n# {nameof(defaultTboxSettings.TypeUris)}: ", cSharpGraph.OntologySettings.TBoxSettings.TypeUris.Select(KV => KV.Key.AssemblyQualifiedName + "; " + KV.Value.ToString()))}";
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.TypeUris)}: " + typeUriString);
            }

            string tBoxSettingsString = tBoxSettingsStringBuilder.ToString();
            if (tBoxSettingsString.Any())
            {
                TTL.AppendLine($"# {nameof(TBoxSettings)}:");
                TTL.AppendLine(tBoxSettingsString);
                TTL.AppendLine();
            }
        }

        // Adds TBox Settings in a human-readable way. The settings are however serialized-deserialized from a specific comment in the header.
        private static void AddABoxSettings(CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings, StringBuilder TTL)
        {
            ABoxSettings defaultAboxSettings = new ABoxSettings();
            StringBuilder aBoxSettingsStringBuilder = new StringBuilder();

            if (cSharpGraph.AllIndividuals?.Any() ?? false)
            {
                TTL.Append("Individuals".TTLSectionTitle());

                if (cSharpGraph.OntologySettings.ABoxSettings.IndividualsBaseAddress != defaultAboxSettings.IndividualsBaseAddress)
                    aBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultAboxSettings.IndividualsBaseAddress)}: " + cSharpGraph.OntologySettings.ABoxSettings.IndividualsBaseAddress);

                if (cSharpGraph.OntologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues != defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)
                    aBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)}: " + cSharpGraph.OntologySettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues);

                string aBoxSettingsString = aBoxSettingsStringBuilder.ToString();
                if (aBoxSettingsString.Any())
                {
                    TTL.AppendLine($"# {nameof(ABoxSettings)}:");
                    TTL.AppendLine(aBoxSettingsString);
                    TTL.AppendLine();
                }
            }
        }
    }
}
