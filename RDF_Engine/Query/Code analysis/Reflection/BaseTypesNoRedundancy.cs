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

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        public static Dictionary<Type, List<Type>> m_cachedBaseTypesNoRedundancy = new Dictionary<Type, List<Type>>();

        [Description("Gets all implemented interfaces and any base type of a given type.\n" +
            "For parent types that implement types that also the child type implements, only the parent type is reported.")]
        public static List<Type> BaseTypesNoRedundancy(this Type sourceType)
        {
            List<Type> baseTypesNoRedundancy = null;
            if (m_cachedBaseTypesNoRedundancy.TryGetValue(sourceType, out baseTypesNoRedundancy))
                return baseTypesNoRedundancy;

            List<Type> baseTypes = sourceType.GetInterfaces().ToList();
            if (sourceType.BaseType != null)
            {
                baseTypes.Add(sourceType.BaseType);
                baseTypes.AddRange(sourceType.BaseType.GetInterfaces());
            }

            // Remove duplicates collected so far
            baseTypes = baseTypes.Distinct().ToList();

            baseTypesNoRedundancy = new List<Type>(baseTypes);

            foreach (Type baseType in baseTypes)
            {
                var baseTypeInterfaces = baseType.GetInterfaces();
                foreach (var baseTypeInterface in baseTypeInterfaces)
                {
                    baseTypesNoRedundancy.Remove(baseTypeInterface);
                }
            }

            m_cachedBaseTypesNoRedundancy[sourceType] = baseTypesNoRedundancy;

            return baseTypesNoRedundancy;
        }

    }
}
