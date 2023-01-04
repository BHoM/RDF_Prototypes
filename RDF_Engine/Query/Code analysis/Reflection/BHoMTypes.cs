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
        private static List<TypeInfo> m_cachedOmTypes = null;

        public static List<TypeInfo> BHoMTypes(this List<Assembly> oMassemblies)
        {
            if (m_cachedOmTypes != null)
                return m_cachedOmTypes;

            // Remove duplicate classes in the same file, e.g. `BH.oM.Base.Output` which has many generics replicas.
            List<TypeInfo> oMTypes = new List<TypeInfo>();
            foreach (Assembly a in oMassemblies)
            {
                if (a == null)
                    continue;

                IEnumerable<TypeInfo> typesDefinedInAssembly = null;

                try
                {
                    typesDefinedInAssembly = a.DefinedTypes?.Where(t => t.IsBHoMType());
                }
                catch (ReflectionTypeLoadException e)
                {
                    Log.RecordError($"Could not load BHoM types from assembly {a.FullName}. Error(s):\n    {string.Join("\n    ", e.LoaderExceptions.Select(le => le.Message))}");
                }

                if (typesDefinedInAssembly != null)
                    oMTypes.AddRange(typesDefinedInAssembly);
            }

            // Sort loaded type by fullname.
            oMTypes = oMTypes.GroupBy(t => t.FullName.OnlyAlphabeticAndDots())
                .Select(g => g.First()).ToList();

            m_cachedOmTypes = oMTypes;

            return oMTypes;
        }
    }
}