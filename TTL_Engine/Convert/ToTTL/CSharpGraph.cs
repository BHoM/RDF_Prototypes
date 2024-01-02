/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.Engine.Adapters.RDF;
using BH.oM.Base;
using BH.oM.Adapters.RDF;
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

namespace BH.Engine.Adapters.TTL
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts a Graph ontological representation of (BHoM) types and their relations into a TTL format.")]
        public static string ToTTL(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings = null, string filepath = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();

            StreamWriter sw = null;
            if (!string.IsNullOrWhiteSpace(filepath))
                sw = new StreamWriter(filepath);

            StringBuilder TTL = null;

            try
            {
                TTL = new StringBuilder();

                TTL.Append(TTLHeader(cSharpGraph.GraphSettings));

                TTL.Append("Annotation Properties".SectionTitle());
                TTL.Append(string.Join("\n", TTLAnnotationProperties()));

                TTL.Append("Datatypes".SectionTitle());
                TTL.Append(string.Join("\n", cSharpGraph.TTLDataTypes(localRepositorySettings)));

                TTL.Append("Classes".SectionTitle());

                AddTBoxSettings(cSharpGraph, TTL);

                TTL.Append(string.Join("\n\n", cSharpGraph.TTLClasses(localRepositorySettings)));

                TTL.Append("Object Properties".SectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLObjectProperties(localRepositorySettings)));

                TTL.Append("Data properties".SectionTitle());
                TTL.Append(string.Join("\n\n", cSharpGraph.TTLDataProperties(localRepositorySettings)));

                AddABoxSettings(cSharpGraph, localRepositorySettings, TTL);

                cSharpGraph.TTLIndividuals(localRepositorySettings, TTL);

                TTL.Append("Footer".SectionTitle());
                TTL.AppendLine($"# {nameof(GraphSettings)}: {cSharpGraph.GraphSettings.ToBase64JsonSerialized()}");
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
            if (cSharpGraph.GraphSettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes != defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes)}: " + cSharpGraph.GraphSettings.TBoxSettings.TreatCustomObjectsWithTypeKeyAsCustomObjectTypes);

            if (cSharpGraph.GraphSettings.TBoxSettings.DefaultBaseUriForUnknownTypes != defaultTboxSettings.DefaultBaseUriForUnknownTypes)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.DefaultBaseUriForUnknownTypes)}: " + cSharpGraph.GraphSettings.TBoxSettings.DefaultBaseUriForUnknownTypes);

            if (cSharpGraph.GraphSettings.TBoxSettings.CustomobjectsTypeKey != defaultTboxSettings.CustomobjectsTypeKey)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.CustomobjectsTypeKey)}: " + cSharpGraph.GraphSettings.TBoxSettings.CustomobjectsTypeKey);

            if (cSharpGraph.GraphSettings.TBoxSettings.CustomObjectTypesBaseAddress != defaultTboxSettings.CustomObjectTypesBaseAddress)
                tBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultTboxSettings.CustomObjectTypesBaseAddress)}: " + cSharpGraph.GraphSettings.TBoxSettings.CustomObjectTypesBaseAddress);

            if (cSharpGraph.GraphSettings.TBoxSettings.TypeUris?.Any() ?? false)
            {
                string typeUriString = $@"{string.Join($"\n# {nameof(defaultTboxSettings.TypeUris)}: ", cSharpGraph.GraphSettings.TBoxSettings.TypeUris.Select(KV => KV.Key.AssemblyQualifiedName + "; " + KV.Value.ToString()))}";
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
                TTL.Append("Individuals".SectionTitle());

                if (cSharpGraph.GraphSettings.ABoxSettings.IndividualsBaseAddress != defaultAboxSettings.IndividualsBaseAddress)
                    aBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultAboxSettings.IndividualsBaseAddress)}: " + cSharpGraph.GraphSettings.ABoxSettings.IndividualsBaseAddress);

                if (cSharpGraph.GraphSettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues != defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)
                    aBoxSettingsStringBuilder.AppendLine($"# {nameof(defaultAboxSettings.ConsiderNullOrEmptyPropertyValues)}: " + cSharpGraph.GraphSettings.ABoxSettings.ConsiderNullOrEmptyPropertyValues);

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

