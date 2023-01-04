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
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Returns the text included in the DescriptionAttribute of a Type, if present.")]
        public static string DescriptionInAttribute(this Type t)
        {
            try
            {
                CustomAttributeData classDescriptionAttribute = t.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(DescriptionAttribute));
                string classDescription = classDescriptionAttribute?.ConstructorArguments.FirstOrDefault().Value.ToString();

                return classDescription;
            }
            catch
            {
                return "";
            }
        }

        [Description("Returns the text included in the DescriptionAttribute of a PropertyInfo, if present.")]
        public static string DescriptionInAttribute(this PropertyInfo pi)
        {
            try
            {
                CustomAttributeData descriptionAttribute = pi.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(DescriptionAttribute));
                string description = descriptionAttribute?.ConstructorArguments.FirstOrDefault().Value.ToString();

                return description;
            }
            catch
            {
                return "";
            }
        }
    }
}
