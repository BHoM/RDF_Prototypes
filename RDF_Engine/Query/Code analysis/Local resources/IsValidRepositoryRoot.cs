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
        private static HashSet<string> m_validRepoRootPathsFound = new HashSet<string>();

        [Description("Checks whether the input path points to a Git root path containing a BHoM repository clone." +
            "A valid 'repository root path' points to a directory that contains, among other repositories, also the BHoM repository.")]
        public static bool IsValidRepositoryRootPath(this string repositoryRootPath)
        {
            if (string.IsNullOrWhiteSpace(repositoryRootPath) || !Directory.Exists(repositoryRootPath))
                return false;

            if (m_validRepoRootPathsFound.Contains(repositoryRootPath))
                return true;

            var allRepoDirectories = Directory.GetDirectories(repositoryRootPath);

            var bhomRepoDirectory = allRepoDirectories.Where(d => d.EndsWith("BHoM")).FirstOrDefault();
            if (bhomRepoDirectory != null && Directory.GetFiles(bhomRepoDirectory).Where(f => f.EndsWith("BHoM.sln")).Count() == 1)
            {
                m_validRepoRootPathsFound.Add(repositoryRootPath);
                return true;
            }

            return false;
        }
    }
}
