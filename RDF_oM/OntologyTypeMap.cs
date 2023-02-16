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

using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapters.RDF
{
    public static class OntologyDataTypeMap
    {
        public static Dictionary<Type, string> ToOntologyDataType = new Dictionary<Type, string>()
        {
            { typeof(string), "xsd:string" },
            { typeof(bool), "xsd:boolean" },
            { typeof(int), "xsd:integer" },
            { typeof(double), "xsd:double" },
            { typeof(float), "xsd:float" },
            { typeof(decimal), "xsd:decimal" },
            { typeof(Guid), "xsd:string"}
        };

        public static Dictionary<string, Type> FromOntologyDataType = new Dictionary<string, Type>()
        {
            { "string",typeof(string) },
            { "boolean", typeof(bool)},
            { "integer" , typeof(int)},
            { "double", typeof(double) },
            { "float" , typeof(float)},
            { "decimal", typeof(decimal)}
        };
    }
}
