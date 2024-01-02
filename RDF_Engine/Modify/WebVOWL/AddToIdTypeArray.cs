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

using BH.oM.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;


namespace BH.Engine.Adapters.RDF
{
    public static partial class Modify
    {
        /***************************************************/

        // This can add data to the `class` and `property` arrays.
        public static void AddToIdTypeArray(this JArray idTypeArray, string id, string type)
        {
            // CLASS
            JObject classObj = new JObject();
            classObj.Add(new JProperty("id", id));
            classObj.Add(new JProperty("type", type));

            idTypeArray.Add(classObj);
        }

        /***************************************************/

        // This can add data to the `classAttribute` and `propertyAttribute` arrays.
        public static void AddToAttributeArray(this JArray attributeArray, string id, Uri uri, string label_en, bool isExternal = false,
            List<string> attributes = null,
            string comment = null,
            List<string> domain = null, List<string> range = null,
            string label_iriBased = null)
        {
            JObject attributeArrayObj = new JObject();
            attributeArrayObj.Add(new JProperty("id", id));
            attributeArrayObj.Add(new JProperty("iri", uri));

            // // - Label
            if (label_iriBased != null)
            {
                JObject labelObj = new JObject();

                labelObj.Add(new JProperty("IRI-based", label_iriBased)); // not sure what this is for
                labelObj.Add(new JProperty("en", label_en)); //  type.Name + $" ({type.Namespace})"

                attributeArrayObj.Add(new JProperty("label", labelObj));
            }
            else
                attributeArrayObj.Add(new JProperty("label", label_en));

            // // - Attributes
            if (isExternal)
                attributes = attributes == null ? new List<string> { "external" } : attributes.Concat(new List<string> { "external" }).ToList();

            attributeArrayObj.Add(new JProperty("attributes", attributes.ToJArray()));

            // // - Comment
            if (!string.IsNullOrWhiteSpace(comment))
            {
                JObject commentObject = new JObject();
                commentObject.Add(new JProperty("undefined", comment)); // only supports an 'undefined' comment property, it seems.
                attributeArrayObj.Add(new JProperty("comment", commentObject));
            }

            // // - Domain and range
            domain = domain?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            range = range?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            if ((domain?.Any() ?? false) && (range?.Any() ?? false))
            {
                attributeArrayObj.Add(domain.ToJProperty("domain"));
                attributeArrayObj.Add(range.ToJProperty("range"));
            }

            attributeArray.Add(attributeArrayObj);
        }
    }
}

