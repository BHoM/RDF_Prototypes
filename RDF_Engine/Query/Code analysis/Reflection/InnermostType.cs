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
        [Description("Get the most nested type within a generic type. For example, passing a type of `List<List<Bar>>` will return the type of `Bar`." +
            "Supports only generic types and IEnumerables." +
            "If the generic type owns more than one generic argument, returns only the first (e.g. for a key-value collection it will only return the type of the key).")]
        public static Type InnermostType<T>(this T obj)
        {
            Type type = typeof(T);

            if (type == typeof(System.Type) && obj is Type && obj != null)
                type = obj as Type;

            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                    return InnermostType(genericArgs[0]);
            }
            else
            {
                if (obj is IEnumerable)
                {
                    return InnermostType(
                        obj.GetType()
                        .GetInterfaces()
                        .Where(t => t.IsGenericType
                            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Select(t => t.GetGenericArguments()[0]).FirstOrDefault());
                }
            }

            return type;
        }
    }
}

