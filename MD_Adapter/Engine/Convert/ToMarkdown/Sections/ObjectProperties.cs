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

namespace BH.Engine.Adapters.Markdown
{
    public static partial class Convert
    {
        private static List<string> TTLObjectProperties(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLObjectProperties = new List<string>();

            for (int i = 0; i < cSharpGraph.ObjectProperties.Count; i++)
            {
                var rel = cSharpGraph.ObjectProperties.ElementAt(i);

                try
                {
                    string TTLObjectProperty = "";

                    if (rel.RangeType == null || rel.PropertyInfo == null)
                        continue;

                    if (localRepositorySettings.TryComputeURLFromFilePaths)
                    {
                        string propertyURI = rel.PropertyInfo.OntologyURI(cSharpGraph.GraphSettings.TBoxSettings, localRepositorySettings).ToString();
                        TTLObjectProperty += $"\n### {propertyURI}";
                    }
                    TTLObjectProperty += $"\n:{rel.PropertyInfo.UniqueNodeId()} rdf:type owl:ObjectProperty ;";
                    TTLObjectProperty += $"\nrdfs:domain :{rel.DomainClass.UniqueNodeId()} ;";
                    TTLObjectProperty += $"\nrdfs:range :{rel.RangeType.UniqueNodeId()} ;";
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

