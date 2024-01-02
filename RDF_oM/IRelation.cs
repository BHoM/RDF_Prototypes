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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapters.RDF
{
    [Description("Classes implementing this interface represent a relation between a Subject thing and an Object thing." +
        "The class name will represent the relation predicate.")]
    public interface IRelation : IObject
    {
        // We use `System.Object` as a type, instead of `System.Type`, because this way we can use IRelations for:
        // - Ontological relations, i.e. between C# classes. We can call these "Static relations"
        // - Knowledge graph relations, i.e. between C# objects (instances of classes). We can call these "Dynamic relations".
        object Subject { get; set; } 
        object Object { get; set; }

        bool IsBidirectional { get; set; } // If unset, this defaults to false.
    }
}

