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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("This component pushed RDF data in TTL file formate to GraphDB using API calls. To push data to GraphDB press the Button or switch the Toggle to true" +
            "filePathRDFData takes the file path where your RDF data was saved as a TTL file.")]
        public static void PostToRepo(string filePathRDFData, string serverAddress = "http://localhost:7200/", string repositoryName = "BHoMVisualization",
            bool deleteRepository = false, bool run = false)
        {
            if (!run)
            {
                BH.Engine.Base.Compute.RecordWarning("To push data to GraphDB press the Button or switch the Toggle to true");
                return;
            }

            // Documentation in GraphDB: http://localhost:7200/webapi

            // Create Http Client and first Endpoint
            var client = new HttpClient();
            var endpointRepoCreate = new Uri(serverAddress + "rest/repositories/");

            // Get repository config file and turn into HTTP Content
            FileStream file = File.OpenRead(@"C:\ProgramData\BHoM\repo-config.ttl");
            HttpContent fileStreamContent = new StreamContent(file);

            // Add Filetype even necessary?
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("Turtle/ttl");

            // Create new formData and Add the Config File to it as name config (http://localhost:7200/webapi)
            var formData = new MultipartFormDataContent();
            formData.Add(fileStreamContent, name: "config", fileName: "repo-config.ttl");

            // Post Respository Request
            var result = client.PostAsync(endpointRepoCreate, formData).Result;
            var json = result.Content.ReadAsStringAsync().Result;


            // Post Data to Repository (also update data)
            String ttlBHoMFile = File.ReadAllText(filePathRDFData);
            StringContent ttlFile = new StringContent(ttlBHoMFile);
            ttlFile.Headers.ContentType = new MediaTypeHeaderValue("text/turtle");

            var endpointRepoPostData = new Uri(serverAddress + "repositories/" + repositoryName + "/statements");
            var resultData = client.PutAsync(endpointRepoPostData, ttlFile).Result;
            var jsonData = result.Content.ReadAsStringAsync().Result;


            if (deleteRepository)
            {
                // HTTP Delete Request, new endpoint (with {RepositoryID} at the end)
                var endpointDeletRepo = new Uri(serverAddress + "rest/repositories/" + repositoryName);
                var result2 = client.DeleteAsync(endpointDeletRepo).Result;
                var json2 = result.Content.ReadAsStringAsync().Result;
            }

        }
    }
}
