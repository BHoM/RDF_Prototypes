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
        private static string m_repositoryRoot = null;

        [Description("Tries to get the root of git repositories from various common locations." +
            "A valid git root path is a directory that contains, among other repositories, also the BHoM repository.")]
        public static bool TryGetRepositoryRootPath(out string repositoryRoot)
        {
            if (!string.IsNullOrWhiteSpace(m_repositoryRoot))
            {
                repositoryRoot = m_repositoryRoot;
                return true;
            }

            repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            // Look in Documents
            repositoryRoot = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).ToString(), Environment.UserName, "Documents", "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            // Try removing special chars from username
            repositoryRoot = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).ToString(), Environment.UserName.OnlyAlphabetic(), "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            // Try removing special chars from username and look in Documents
            repositoryRoot = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).ToString(), Environment.UserName.OnlyAlphabetic(), "Documents", "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            return false;
        }
    }
}
