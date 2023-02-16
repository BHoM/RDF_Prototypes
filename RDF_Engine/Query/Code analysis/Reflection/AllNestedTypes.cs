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
        public static HashSet<Type> AllNestedTypes(this List<Type> types)
        {
            return new HashSet<Type>(types.SelectMany(t => t.AllNestedTypes()));
        }

        public static HashSet<Type> AllNestedTypes(this Type type)
        {
            HashSet<Type> result = new HashSet<Type>();

            AllNestedTypes(type, result);

            return result;
        }

        private static void AllNestedTypes(this Type type, HashSet<Type> result)
        {
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                Type propertyType = prop.PropertyType;

                if (propertyType.IsBHoMType())
                {
                    // Add type to result
                    result.Add(propertyType);

                    // Recurse on sub-properties
                    propertyType.GetProperties().ToList().ForEach(p => AllNestedTypes(p.PropertyType, result));
                }

                if (propertyType.IsGenericType)
                {
                    List<Type> genericArgumentTypes = propertyType.GetGenericArguments().ToList();

                    List<Type> BHoMGenericArgumentTypes = genericArgumentTypes.Where(t => t.IsBHoMType()).ToList();

                    // Add type to result
                    BHoMGenericArgumentTypes.ForEach(bt => result.Add(bt));

                    // Recurse on generic types
                    // TODO: This is limited to generic arguments that are bhom types.
                    // We can expand to also consider generic arguments that are themselves generic types,
                    // e.g. to get also the `Bar` in a type `Dictionary<string, List<Bar>>`.
                    BHoMGenericArgumentTypes.ForEach(bt => AllNestedTypes(bt, result));
                }
            }
        }
    }
}
