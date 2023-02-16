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
        [Description("Returns the 'namespace group' of a certain type. This is the topmost portion of the namespace to which the type belongs." +
            "Examples: a type `BH.oM.Structure.Element.Bar` is in the namespace group `BH.oM.Structure`." +
            "A type of `BH.oM.Adapters.SAP2000.Elements.SomeSapType` belongs to the namespace group `BH.oM.Adapters.SAP2000`.")]
        public static string NamespaceGroup(this Type t, int namespaceGroupDepth = 3)
        {
            // Group by namespace
            if (namespaceGroupDepth < 3)
                namespaceGroupDepth = 3; // at least group per BH.oM.Something

            string ns = t.Namespace;

            if (ns.StartsWith("BH.oM.Adapters") || ns.StartsWith("BH.oM.External"))
                namespaceGroupDepth = 4; // at least group per BH.oM.Adapters.Something

            string namespaceGroup = string.Join(".", t.Namespace.Split('.').Take(namespaceGroupDepth));

            return namespaceGroup;
        }
    }
}