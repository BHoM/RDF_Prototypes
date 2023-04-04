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

using BH.Engine.Adapters.RDF;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Shacl.Validation;

namespace BH.Engine.Adapters.GraphDB
{
    public static partial class Compute
    {
        [Description("Push RDF data from a TTL file to GraphDB using its REST API.")]
        [Input("TTLfilePath", "Turtle file where your RDF data was saved.")]
        [Input("serverAddress", "Localhost address where GraphDB is exposed. This can be changed from GraphDB settings file.")]
        [Input("repositoryName", "GraphDB repository name where the graph data is stored.")]
        [Input("run", "Activate the push.")]
        public static bool PostToRepo(string TTLfilePath, string serverAddress = "http://localhost:7200/", string repositoryName = "BHoMVisualization", bool run = false)
        {
            if (!run)
            {
                Log.RecordWarning("To push data to GraphDB press the Button or switch the Toggle to true");
                return false;
            }

            // Documentation in GraphDB: http://localhost:7200/webapi

            var httpClient = new HttpClient();

            // Set the GraphDB REST API URL for creating a repository
            string apiUrl = $"{serverAddress}rest/repositories";


            // Read the configuration template
            string configTemplate = File.ReadAllText(@"C:\ProgramData\BHoM\repo-config.ttl");
            string config = configTemplate.Replace("{{REPOSITORY_ID}}", repositoryName);

            //// Read the configuration template
            HttpContent fileStreamContent = new StringContent(config, Encoding.UTF8, "application/xml");

            // Create the request content
            var formData = new MultipartFormDataContent();
            formData.Add(fileStreamContent, name: "config", fileName: "repo-config.ttl");

            // Send a POST request to create the repository
            HttpResponseMessage response = httpClient.PostAsync(apiUrl, formData).Result;

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
                Console.WriteLine($"Repository '{repositoryName}' has been created.");
            else
                Console.WriteLine($"Failed to create repository '{repositoryName}': {response.ReasonPhrase}");


            // Post Data to Repository (also update data)
            String ttlBHoMFile = File.ReadAllText(TTLfilePath);
            StringContent ttlFile = new StringContent(ttlBHoMFile);
            ttlFile.Headers.ContentType = new MediaTypeHeaderValue("text/turtle");

            var endpointRepoPostData = new Uri(serverAddress + "repositories/" + repositoryName + "/statements");
            var resultData = httpClient.PutAsync(endpointRepoPostData, ttlFile).Result;
            string jsonData = resultData.Content.ReadAsStringAsync().Result;
            return resultData.IsSuccessStatusCode;
        }
    }
}
