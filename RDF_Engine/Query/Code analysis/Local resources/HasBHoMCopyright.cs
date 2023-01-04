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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Checks if this assembly has copyright information, and if that contains 'BHoM' as substring." +
            "Useful to distinguish BHoM VS. non-BHoM assemblies contained in a folder.")]
        public static bool HasBHoMCopyright(Assembly assembly)
        {
            string copyright = "";
            try
            {
                object[] attribs = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
                if (attribs.Length > 0)
                {
                    copyright = ((AssemblyCopyrightAttribute)attribs[0]).Copyright;
                }

                return copyright.Contains("BHoM");
            }
            catch
            {
                BH.Engine.Base.Compute.RecordError($"Could not obtain copyright information for assembly {assembly.GetName().Name}");
            }

            return false;
        }
    }
}
