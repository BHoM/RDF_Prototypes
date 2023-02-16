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
    public static partial class Query
    {
        public static SortedDictionary<string, List<TypeInfo>> OMTypesPerNamespace(List<TypeInfo> oMTypes, List<string> namespaceToConsider = null, List<string> typeNamesToConsider = null, int namespaceGroupDepth = 3)
        {
            // Null check
            oMTypes = oMTypes.Where(t => t != null && t.Namespace != null).ToList();

            // Filters
            if (namespaceToConsider != null && namespaceToConsider.All(f => !string.IsNullOrWhiteSpace(f)))
                oMTypes = oMTypes.Where(t => namespaceToConsider.Any(nsf =>
                {
                    string typeFullName = t.FullNameValidChars();
                    if (nsf.StartsWith("BH."))
                        return typeFullName.StartsWith(nsf);
                    else
                        return typeFullName.Contains(nsf);
                })).ToList();


            if (typeNamesToConsider != null && typeNamesToConsider.All(f => !string.IsNullOrWhiteSpace(f)))
                oMTypes = oMTypes.Where(t => typeNamesToConsider.Any(tn =>
                {
                    if (tn.StartsWith("BH."))
                        return t.FullNameValidChars() == tn;
                    else
                        return t.NameValidChars().Contains(tn);
                })).ToList();


            Dictionary<string, List<TypeInfo>> oMTypesPerNamespace = oMTypes.GroupBy(t => t.NamespaceGroup()).ToDictionary(g => g.Key, g => g.ToList());

            return new SortedDictionary<string, List<TypeInfo>>(oMTypesPerNamespace, new NaturalSortComparer<string>());
        }
    }
}