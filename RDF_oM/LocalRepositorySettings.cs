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

using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapters.RDF
{
    public class LocalRepositorySettings : IObject
    {
        public bool TryComputeURLFromFilePaths { get; set; } = true;
        [Description("Path to the root of your local Git folder where the BHoM repository can be found. For example, `C:\\Users\\myUserName\\GitHub`.")]
        public string GitRootPath { get; set; } = "";
        public string CacheRootPath { get; set; } = Directory.GetParent(Directory.GetParent(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).FullName).FullName;
        public string CacheFileName_RepositoryAllFilePaths { get; set; } = "cached_RepositoryAllFilePaths.txt";
        public string CacheFileName_PropertyInfoFileLines { get; set; } = "cached_PropertyInfoFileLines.txt";
        public string SaveDir_RelativeToRoot = "WebVOWLOntology";
        public bool ReadCacheFiles { get; set; } = true;
        public bool WriteCacheFiles { get; set; } = false;
    }
}
