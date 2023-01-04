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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Returns the Full Name of the input Type, only including characters that are alphanumeric, dots and/or greek letters (which are useful for BH.oM.Structure objects properties).")]
        public static string FullNameValidChars(this PropertyInfo pi)
        {
            return $"{FullNameValidChars(pi.DeclaringType)}.{pi.Name}";
        }

        [Description("Returns the Full Name of the input Type, only including characters that are alphanumeric, dots and/or greek letters (which are useful for BH.oM.Structure objects properties).")]
        public static string FullNameValidChars(this Type type)
        {
            string fullNamePlusReplaced = type.FullName.Replace('+', '.');

            return RemoveInvalidChars(fullNamePlusReplaced ?? $"{type.Namespace}.{type.Name}");
        }

        /***************************************************/

        [Description("Returns the Name of the input PropertyInfo, only including characters that are alphanumeric, dots and/or greek letters (which are useful for BH.oM.Structure objects properties).")]
        public static string NameValidChars(this PropertyInfo pi)
        {
            return RemoveInvalidChars(pi.Name);
        }
        
        [Description("Returns the Name of the input Type, only including characters that are alphanumeric, dots and/or greek letters (which are useful for BH.oM.Structure objects properties).")]
        public static string NameValidChars(this Type type)
        {
            return RemoveInvalidChars(type.Name);
        }


        /***************************************************/

        public static string RemoveInvalidChars(string text)
        {
            if (text.Contains("`"))
            {
                // remove those weird chars that sometimes happen e.g. IElementLoad`1
                text = text.Substring(0, text.IndexOf("`"));

                while (Char.IsDigit(text.Last()))
                    text = text.Substring(0, text.Length - 1); // if last char is a number, remove it.
            }

            Regex rgx = new Regex(@"[^a-zA-Z0-9.\p{IsGreek}]"); //only alphanumeric, dots and greek letters (useful for structural props).
            text = rgx.Replace(text, "");

            return text;
        }
    }
}