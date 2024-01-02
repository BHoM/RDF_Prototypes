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

using BH.oM.Adapters.RDF;
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
    public static partial class Compute
    {
        public static SortedDictionary<string, string> WebVOWLJsonPerNamespace(List<TypeInfo> oMTypes, LocalRepositorySettings settings, List<string> namespaceToConsider = null, List<string> typeNamesToConsider = null, int namespaceGroupDepth = 3)
        {
            var oMTypesPerNamespace = Query.OMTypesPerNamespace(oMTypes, namespaceToConsider, typeNamesToConsider, namespaceGroupDepth);

            var res = WebVOWLJsonPerNamespace(oMTypesPerNamespace, settings);

            return res;
        }

        public static SortedDictionary<string, string> WebVOWLJsonPerNamespace(IDictionary<string, List<TypeInfo>> oMTypesGroupsPerNamespace, LocalRepositorySettings settings)
        {
            SortedDictionary<string, string> result = new SortedDictionary<string, string>(new NaturalSortComparer<string>());

            foreach (var kv in oMTypesGroupsPerNamespace)
            {
                // Extract a dictionary representation of the BHoM Ontology Graph
                Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = kv.Value.DictionaryGraphFromTypeInfos();
                string webVOWLJson = Engine.Adapters.RDF.Convert.ToWebVOWLJson(dictionaryGraph, settings, new HashSet<string> { kv.Key });

                result[kv.Key] = webVOWLJson;
            }

            return result;
        }
    }
}
