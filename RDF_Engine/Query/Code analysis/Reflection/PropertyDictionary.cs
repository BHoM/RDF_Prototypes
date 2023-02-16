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

namespace BH.Engine.Adapters.RDF
{
    public static partial class Query
    {
        public static Dictionary<string, object> PropertyDictionary(this object obj, bool onlyDeclaredProps = true)
        {
            if (obj == null)
            {
                Base.Compute.RecordWarning("Cannot query the property dictionary of a null object. An empty dictionary will be returned.");
                return new Dictionary<string, object>();
            }

            Dictionary<string, object> dic = new Dictionary<string, object>();

            PropertyInfo[] properties;

            if (onlyDeclaredProps)
                properties = obj.GetType().DeclaredProperties();
            else
                properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0) continue;
                var value = prop.GetValue(obj, null);
                dic[prop.Name] = value;
            }

            return dic;
        }
    }
}