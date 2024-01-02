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
using BH.oM.Adapters.RDF;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.Markdown
{
    public static partial class Compute
    {
        [Description("Computes a Markdown ontology with the input Types, and writes it to a file. The ontology will include T-Box only." +
             "The T-Box is constructed from the Types of the input objects, and their relations, expressed via the CSharp object properties.")]
        public static void ToMarkdown(this List<Type> types, string filePath, GraphSettings graphSettings = null, LocalRepositorySettings localRepositorySettings = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();
            graphSettings = graphSettings ?? new GraphSettings();

            CSharpGraph cSharpGraph = BH.Engine.Adapters.RDF.Compute.CSharpGraph(types, graphSettings);

            cSharpGraph.ToMarkdown(localRepositorySettings, filePath);
        }


        [Description("Computes a Markdown ontology with the input Types. The ontology will include T-Box only." +
             "The T-Box is constructed from the Types of the input objects, and their relations, expressed via the CSharp object properties.")]
        public static string ToMarkdown(this List<object> objects, GraphSettings graphSettings = null, LocalRepositorySettings localRepositorySettings = null)
        {
            localRepositorySettings = localRepositorySettings ?? new LocalRepositorySettings();
            graphSettings = graphSettings ?? new GraphSettings();

            CSharpGraph cSharpGraph = BH.Engine.Adapters.RDF.Compute.CSharpGraph(objects, graphSettings);

            string TTL = cSharpGraph.ToMarkdown(localRepositorySettings);

            return TTL;
        }
    }
}

