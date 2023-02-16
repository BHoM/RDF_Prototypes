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
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        [Description("Extracts a Dictionary representation of a Graph established by the input types and their code relationships." +
            "The Key of the dictionary is the Type, while the Value is the list of edges (relationships).")]
        public static Dictionary<TypeInfo, List<IRelation>> DictionaryGraphFromTypeInfos(this IEnumerable<TypeInfo> oMTypes)
        {
            Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = new Dictionary<TypeInfo, List<IRelation>>();

            foreach (TypeInfo oMType in oMTypes)
            {
                // Check if this Type has been already added to the Graph, otherwise initialise the edges list.
                List<IRelation> edges = null;
                if (!dictionaryGraph.TryGetValue(oMType, out edges))
                    edges = new List<IRelation>();

                // Parse the existing code-relationships.
                var relationsFromType = oMType.RelationsFromType();
                edges.AddRange(relationsFromType);

                dictionaryGraph[oMType] = edges;
            }

            return dictionaryGraph;
        }
    }
}
