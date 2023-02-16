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
        public static Dictionary<Type, List<Type>> m_cachedBaseTypes = new Dictionary<Type, List<Type>>();

        [Description("Gets all implemented interfaces and any base type of a given type.")]
        public static List<Type> BaseTypes(this Type type)
        {
            List<Type> baseTypes = new List<Type>();

            if (type == null)
                return baseTypes;

            if (m_cachedBaseTypes.TryGetValue(type, out baseTypes))
                return baseTypes;
            else
                baseTypes = new List<Type>();

            baseTypes.AddRange(type.GetInterfaces());

            Type baseType = type.BaseType;
            if (baseType != null)
                baseTypes.Add(baseType);

            m_cachedBaseTypes[type] = baseTypes;

            return baseTypes;
        }
    }
}