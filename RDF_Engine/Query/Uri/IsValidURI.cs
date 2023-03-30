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

using BH.Engine.Base;
using BH.Engine.Adapters.RDF.Types;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        [Description("Checks whether the input string constitute a valid URI scheme")]
        public static bool IsValidURI(string uriString, string appendToMessage = null)
        {
            Uri uriResult;

            // Try to create a Uri object from the input string
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out uriResult))
            {
                Log.RecordError($"Invalid URI string. The URI you provided does not start with a valid scheme. For example the URI can start with http://, https:// or doi://. " + appendToMessage, typeof(ArgumentException));
                return false;
            }

            return true;
        }
    }
}
