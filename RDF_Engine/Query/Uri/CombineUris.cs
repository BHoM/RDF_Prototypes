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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Combine the given strings and base uri in a single absolute Uri. More reliable than standard Uri.Combine.")]
        public static Uri CombineUris(this Uri baseUri, params string[] uris)
        {
            if (baseUri == null)
                return null;

            string combinedUriString = baseUri.ToString();

            if (combinedUriString.EndsWith("/"))
                combinedUriString = combinedUriString.Remove(combinedUriString.Length - 1, 1);

            foreach (string uri in uris)
            {
                if (uri == null)
                    continue;

                string correctedUri = uri.Replace(@"\", "/");
                if (correctedUri.StartsWith("/"))
                    correctedUri = correctedUri.Remove(0, 1);

                if (correctedUri.EndsWith("/"))
                    correctedUri = correctedUri.Remove(correctedUri.Length - 1, 1);

                combinedUriString += "/" + correctedUri;
            }

            Uri result = null;
            Uri.TryCreate(combinedUriString, UriKind.Absolute, out result);

            return result;
        }

        /***************************************************/

        public static Uri CombineUris(params string[] uris)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder = new UriBuilder(uris.FirstOrDefault());

            return CombineUris(uriBuilder.Uri, uris.Skip(1).ToArray());
        }
    }
}
