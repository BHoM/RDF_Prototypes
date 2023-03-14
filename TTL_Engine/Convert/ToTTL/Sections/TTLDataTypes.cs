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
        private static List<string> TTLDataTypes(this CSharpGraph cSharpGraph, LocalRepositorySettings r)
        {
            List<string> dataTypes = new List<string>();

            dataTypes.Add(DefaultDataTypeForUnknownConversion(cSharpGraph.GraphSettings.TBoxSettings, r));

            return dataTypes;
        }

        private static string DefaultDataTypeForUnknownConversion(TBoxSettings tboxSettings, LocalRepositorySettings r)
        {
            string defaultDataTypeUri = typeof(BH.oM.Adapters.RDF.Base64JsonSerialized).OntologyUri(tboxSettings, r)?.ToString();

            // TODO: add better guard against null, possibly adding mechanism to provide a defaultDataType URI rather than a Type.
            defaultDataTypeUri = defaultDataTypeUri ?? "https://github.com/BHoM/RDF_Prototypes/commit/ff8ccb68dbba5aeadb4a9a284f141eb1515e169a";

            string TTLDataType = "";
            //TTLDataType = $"### {defaultDataTypeUri}";
            TTLDataType += $"\n<https://github.com/BHoM/RDF_Prototypes/blob/main/RDF_oM/Base64JsonSerialized.cs> rdf:type rdfs:Datatype ;";
            TTLDataType += "\n" + $@"rdfs:label ""{typeof(BH.oM.Adapters.RDF.Base64JsonSerialized).DescriptiveName()}""@en .";

            return TTLDataType;
        }
    }
}
