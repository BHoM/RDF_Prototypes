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

using BH.oM.Base.Attributes;
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

        [Description("Gets all base classes and implemented interfaces of a given type." +
            "The order is important: the output list returns first all inherited base classes, then all implemented interfaces." +
            "This method does not recurse for the parent base types. The method performs caching for faster retrievals after the first.")]
        [Input("type", "Input type.")]
        [Output("baseTypes", "List of all inherited base classes, followed by all implemented interfaces.")]
        public static List<Type> BaseTypes(this Type type)
        {
            List<Type> baseTypes = new List<Type>();

            if (type == null)
                return baseTypes;

            if (m_cachedBaseTypes.TryGetValue(type, out baseTypes))
                return baseTypes;
            else
                baseTypes = new List<Type>();

            Type baseType = type.BaseType;
            if (baseType != null)
                baseTypes.Add(baseType);

            baseTypes.AddRange(type.GetInterfaces());

            m_cachedBaseTypes[type] = baseTypes;

            return baseTypes;
        }
    }
}
