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

using BH.oM.Base;
using BH.oM.Adapters.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        [Description("Count the number of occurrencies of a substring in a text.")]
        [Input("text", "Text where to look for occurrencies of a substring.")]
        [Input("substring", "Substring to look for in the text.")]
        [Output("count", "Number of occurrencies of the substring in the text.")]
        public static int CountSubstring(this string text, string substring)
        {
            int count = 0, minIndex = text.IndexOf(substring, 0);
            while (minIndex != -1)
            {
                minIndex = text.IndexOf(substring, minIndex + substring.Length);
                count++;
            }

            return count;
        }
    }
}
