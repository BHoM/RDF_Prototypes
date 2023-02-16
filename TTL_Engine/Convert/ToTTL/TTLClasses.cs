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

using BH.Engine.RDF;
using BH.oM.RDF;
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
        private static List<string> TTLClasses(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLClasses = new List<string>();

            foreach (var classType in cSharpGraph.Classes)
            {
                string TTLClass = "";

                // Declaration with Uri
                string typeUri = classType.OntologyUri(cSharpGraph.OntologySettings.TBoxSettings, localRepositorySettings).ToString();
                TTLClass += $"### {typeUri}";

                // Class Identifier
                TTLClass += $"\n:{classType.UniqueNodeId()} rdf:type owl:Class;";

                // Subclasses
                List<Type> parentTypes = classType.BaseTypesNoRedundancy().Where(t => t.IsOntologyClass(cSharpGraph.OntologySettings.TBoxSettings)).ToList();

                foreach (Type subClass in parentTypes)
                {
                    TTLClass += $"\n\t\trdfs:subClassOf :{subClass.UniqueNodeId()};";
                }

                // Class label
                TTLClass += "\n\t\t" + $@"rdfs:label ""{classType.DescriptiveName()}""@en .";

                TTLClasses.Add(TTLClass);
            }

            return TTLClasses;
        }
    }
}
