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
        [Description("If the input type is a Generic type, looks recurively in its generic Arguments to see if a BHoM type can be found." +
            "I.e. if any argument is a generic argument, recurses until a non-generic argument is found, which is tested for being a BHoM type.")]
        public static bool IsGenericTypeWithBHoMArgs(this Type t)
        {
            if (!t.IsGenericType)
                return false;

            return RecurseGenericsForBHoMArg(t);
        }

        private static bool RecurseGenericsForBHoMArg(this Type t)
        {
            if (t.IsBHoMType())
                return true;

            List<Type> genericArgs = t.GetGenericArguments().ToList();

            foreach (Type type in genericArgs)
            {
                if (type.RecurseGenericsForBHoMArg())
                    return true;
            }

            return false;
        }
    }
}

