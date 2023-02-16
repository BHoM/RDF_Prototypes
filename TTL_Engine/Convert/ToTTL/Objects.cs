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


using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.RDF;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.TTL
{
    public static partial class Compute
    {
        [Description("Computes a TTL ontology with the input IObjects. The ontology will include both T-Box and A-Box." +
             "The T-Box is constructed from the Types of the input objects, and their relations, expressed via the CSharp object properties.")]
        public static void ToTTL(this List<object> objects, string filePath, OntologySettings ontologySettings = null, LocalRepositorySettings localRepositorySettings = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();
            ontologySettings = ontologySettings ?? new OntologySettings();

            CSharpGraph cSharpGraph = Engine.Adapters.RDF.Compute.CSharpGraph(objects, ontologySettings);

            cSharpGraph.ToTTL(localRepositorySettings, filePath);
        }


        [Description("Computes a TTL ontology with the input IObjects. The ontology will include both T-Box and A-Box." +
            "The T-Box is constructed from the Types of the input objects, and their relations, expressed via the CSharp object properties.")]
        public static string ToTTL(this List<object> objects, OntologySettings ontologySettings = null, LocalRepositorySettings localRepositorySettings = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();
            ontologySettings = ontologySettings ?? new OntologySettings();

            CSharpGraph cSharpGraph = Engine.Adapters.RDF.Compute.CSharpGraph(objects, ontologySettings);

            string TTL = cSharpGraph.ToTTL(localRepositorySettings);

            return TTL;
        }
    }
}
