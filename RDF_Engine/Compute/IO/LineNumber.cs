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

using BH.Engine.Adapters.RDF.Types;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.RDF;
using Newtonsoft.Json;
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
    public static partial class Compute
    {
        private static Dictionary<string, List<string>> m_cachedFileLines = new Dictionary<string, List<string>>();
        private static Dictionary<PropertyInfo, int> m_cachedPinfoFileline = new Dictionary<PropertyInfo, int>();

        [Description("Looks for the line number of a property in its `.cs` file by reading the file. Returns -1 if not found. 0 indicates the first line." +
            "LocalRepositorySettings can be input to specify how to cache this operation to speed it up after its first computation.")]
        public static int LineNumber(PropertyInfo pi, LocalRepositorySettings settings)
        {
            // Null guards
            if (pi == null || pi is CustomPropertyInfo || !pi.DeclaringType.IsBHoMType())
                return -1;

            settings = settings ?? new LocalRepositorySettings();

            // Custom Type guard
            if (typeof(CustomObjectType).IsAssignableFrom(pi.DeclaringType))
            {
                Log.RecordError($"Can not compute the code line number of property `{pi.Name}` that is of a {nameof(CustomObjectType)} type.", true);
                return -1;
            }

            int index = -1;

            if (settings.ReadCacheFiles && ReadCache_PInfoFileline(settings) && m_cachedPinfoFileline.TryGetValue(pi, out index))
                return index;

            string declaringTypeFilePath = pi.DeclaringType.FilePathFromLocalRepository(settings, false);

            List<string> lines = null;

            if (!string.IsNullOrWhiteSpace(declaringTypeFilePath) && !m_cachedFileLines.TryGetValue(declaringTypeFilePath, out lines))
            {
                lines = File.ReadAllLines(declaringTypeFilePath).ToList();
                m_cachedFileLines[declaringTypeFilePath] = lines;
            }

            // Get the index from the read lines of the file.
            index = lines?.FindIndex(l =>
            l.Contains(pi.Name) &&
            l.ToLower().Contains(pi.PropertyType.GetCodeName().ToLower()) &&
            //l.Contains("public") &&
            l.Contains("{"))
                ?? -1;

            if (index == -1)
                Log.RecordWarning($"Could not find Line Number of property {pi.FullNameValidChars()}");
            else
                m_cachedPinfoFileline[pi] = index;

            return index;
        }

        private static bool ReadCache_PInfoFileline(LocalRepositorySettings settings)
        {
            if (m_cachedPinfoFileline != null && m_cachedPinfoFileline.Any())
                return true;

            Dictionary<PropertyInfo, int> read = null;

            try
            {
                string path = Path.Combine(settings.CacheRootPath, settings.CacheFileName_PropertyInfoFileLines);
                read = JsonConvert.DeserializeObject<Dictionary<PropertyInfo, int>>(path);
            }
            catch
            {
                return false;
            }

            if (read == null)
                return false;

            m_cachedPinfoFileline = read;

            return true;
        }

        public static bool WriteCache_PInfoFileline(LocalRepositorySettings settings)
        {
            if (m_cachedPinfoFileline == null && !m_cachedPinfoFileline.Any())
                return false;

            string path = Path.Combine(settings.CacheRootPath, settings.CacheFileName_PropertyInfoFileLines);
            File.WriteAllText(path, JsonConvert.SerializeObject(m_cachedPinfoFileline));

            return true;
        }

        public static int LineNumber(MemberInfo mi, LocalRepositorySettings settings)
        {
            string declaringTypeFilePath = mi.DeclaringType.FilePathFromLocalRepository(settings);

            List<string> lines = null;

            if (!m_cachedFileLines.TryGetValue(declaringTypeFilePath, out lines))
            {
                lines = File.ReadAllLines(declaringTypeFilePath).ToList();
                m_cachedFileLines[declaringTypeFilePath] = lines;
            }

            int index = lines.FindIndex(l => l.Contains(mi.Name));

            return index;
        }
    }
}
