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

using System;
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
        private static Dictionary<Assembly, string> m_CachedToolkitNames = new Dictionary<Assembly, string>();

        public static bool IsToolkitAssembly(this Assembly assembly, out string toolkitName, string dllDirectory = @"C:\ProgramData\BHoM\Assemblies")
        {
            toolkitName = null;

            if (m_CachedToolkitNames.TryGetValue(assembly, out toolkitName))
                return true;

            var assemblies = Compute.LoadAssembliesInDirectory(dllDirectory, true, false);
            var toolkitAssemblies = RDF.Query.ToolkitAssemblies(assemblies);

            foreach (var kv in toolkitAssemblies)
            {
                if (kv.Value.Contains(assembly))
                {
                    toolkitName = kv.Key;
                    m_CachedToolkitNames[assembly] = toolkitName;
                    return true;
                }
            }

            return false;
        }
    }
}

