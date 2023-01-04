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
        public static bool IsOntologyClass(this Type type)
        {
            // This method answers this question:
            // is there (should there be) a Class in the ontology that corresponds to this Type?
            // In general, this method should return true for BHoM types (e.g. Column, Bar, etc.), because they mostly correspond to Ontological classes.
            // However, there are particular cases to consider.

            if (type == null)
                return false;

            if (type.IsBHoMType())
            {
                // Not all BHoM types correspond to Ontology classes.
                // Example: IBHoMObject.FragmentSet:
                // FragmentSet is a BHoM type, but it is a collection of other objects.
                // We need to define how we want to treat collectinos; in the meantime, we tranlsate it to a Data property ("SerialisedJson").
                // ==> Other exceptions like this must be captured here.

                if (typeof(FragmentSet).IsAssignableFrom(type))
                    return false;

                return true;
            }

            // Keep the following section verbose as it is; easier debugging.
            if (type.IsClass)
                if (type.Module.ScopeName != "CommonLanguageRuntimeLibrary")
                    return true;  
                else
                    return false; // do not include System types in Ontology.

            return false;
        }
    }
}
