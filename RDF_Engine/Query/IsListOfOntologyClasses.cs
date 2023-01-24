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

using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static bool IsList(this Type t)
        {
            if (t == null)
                return false;

            return typeof(IList).IsAssignableFrom(t);
        }

        public static bool IsListOfOntologyClasses(this Type sourceType, object sourceObj)
        {
            if (sourceType == null)
                return false;

            // Make sure the type is a List.
            if (!sourceType.IsList())
                return false;

            // Check the List generic argument.
            Type[] genericArgs = sourceType.GetGenericArguments();

            if (genericArgs.Length != 1)
                return false;

            // If the List generic arg can be translated to an Ontology class, job done.
            if (genericArgs.First() != typeof(System.Object))
                return genericArgs.First().IsOntologyClass();

            // If the List generic arg is System.Object, the objects may still be Ontology classes that have been boxed.
            if (sourceObj != null && genericArgs.First() == typeof(System.Object))
            {
                List<object> objList = sourceObj as List<object>;

                // Unbox the objects and see if their actual type is an Ontology class.
                return objList.All(o => o.GetType().IsOntologyClass());
            }

            return false;
        }

        public static bool IsListOfOntologyClasses(this IndividualObjectProperty iop)
        {
            Type rangeType = iop.RangeIndividual?.GetType();

            return IsListOfOntologyClasses(rangeType, iop.RangeIndividual);
        }

        public static bool IsListOfDatatypes(this Type t)
        {
            if (t.IsList())
            {
                Type[] genericArgs = t.GetGenericArguments();

                if (genericArgs.Length == 1 && genericArgs.First().IsDataType())
                    return true;
            }

            return false;
        }
    }
}
