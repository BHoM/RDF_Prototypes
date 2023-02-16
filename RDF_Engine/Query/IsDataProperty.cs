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

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        public static bool IsDataProperty(this PropertyInfo pi, TBoxSettings tBoxSettings)
        {
            // An ontology Data property is a relation from a class of an ontology to a Data Type.
            // A CSharp PropertyInfo can corresponds to an Object Property 
            // if the range of the relation (= the property type) is NOT an ontology class,
            // while the domain of the relation (the PropertyInfo's Declaring Type) is an ontology class.

            return pi.PropertyType.IsDataType(tBoxSettings) && pi.DeclaringType.IsOntologyClass(tBoxSettings);
        }
    }
}
