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
using BH.oM.Adapters.RDF;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        public static string IndividualId(this object individual)
        {
            if (individual == null)
                return null;

            // If it is an IObject, simply use BHoM's Hash method.
            IObject iObject = individual as IObject;
            if (iObject != null)
            {
                string hash = BH.Engine.Base.Query.Hash(iObject);
                return GuidFromString(hash).ToString();

            }

            BHoMObject bHoMObject = individual as BHoMObject;
            if (bHoMObject != null)
                return bHoMObject.BHoM_Guid.ToString();

            Log.RecordError("Could not query the ID for an individual.");
            return null;
        }
    }
}
