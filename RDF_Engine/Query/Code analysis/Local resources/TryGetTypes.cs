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

using BH.Engine.Base;
using BH.Engine.RDF.Types;
using BH.oM.RDF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Log = BH.Engine.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Type[] TryGetTypes(this Assembly a, bool logWarning = true)
        {
            try
            {
                return a.GetTypes();
            }
            catch(ReflectionTypeLoadException ex)
            {
                if (logWarning)
                    Log.RecordWarning($"Could not load types from Assembly {a.FullName}. Exception(s):\n{ex.LoaderExceptions.Select(e => e.Message.SplitInLinesAndTabify())}", doNotRepeat: true);
            }
            return new Type[] { };
        }

    }
}
