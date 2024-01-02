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

using BH.Engine.Adapters.RDF;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.TTL
{
    public static partial class Convert
    {
        private static List<string> TTLDataProperties(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLDataProperties = new List<string>();

            for (int i = 0; i < cSharpGraph.DataProperties.Count; i++)
            {
                var rel = cSharpGraph.DataProperties.ElementAt(i);

                // Ontology URI not used for Data Types.
                //string ontologyUri = rel.PropertyInfo.OntologyURI(cSharpGraph.OntologySettings.TBoxSettings, localRepositorySettings)?.ToString();

                //if (ontologyUri.IsNullOrEmpty())
                //{
                //    Log.RecordWarning($"Could not add the DataProperty relation `{rel.PropertyInfo.Name}` of type `{rel.PropertyInfo.DeclaringType}`: could not compute its URI.");
                //    continue;
                //}

                try
                {
                    string TTLDataProperty = "";

                    string propertyURI = rel.PropertyInfo.OntologyURI(cSharpGraph.GraphSettings.TBoxSettings, localRepositorySettings)?.ToString();
                    if (!string.IsNullOrWhiteSpace(propertyURI))
                        TTLDataProperty += $"\n### {propertyURI}";

                    TTLDataProperty += $"\n:{rel.PropertyInfo.UniqueNodeId()} rdf:type owl:DatatypeProperty ;";
                    TTLDataProperty += $"\nrdfs:domain :{rel.DomainClass.UniqueNodeId()} ;";

                    if (rel.RangeType == null || rel.PropertyInfo == null)
                        continue;

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
            string defaultDataType_IObjectProperty = "\n" + $@"###  {defaultTypeForUnknownConversions.OntologyUri(cSharpGraph.GraphSettings.TBoxSettings, localRepositorySettings)}
                {defaultTypeForUnknownConversions.DescriptiveName()} rdf:type owl:DatatypeProperty ;
                rdfs:domain :IObject ;
                rdfs:range {defaultTypeForUnknownConversions.UniqueNodeId()} .";

            return TTLDataProperties;
        }
    }
}

