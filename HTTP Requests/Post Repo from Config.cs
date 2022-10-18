using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace HTTP_Requests
{
    internal class Class1
    {
        static void Main(string[] args)
        {
            // Documentation: http://localhost:7200/webapi

            // Create Http Client and first Endpoint
            var client = new HttpClient();
            var endpoint = new Uri("http://localhost:7200/rest/repositories/");

            // Get repository config file and turn into HTTP Content
            FileStream file = File.OpenRead(@"G:\My Drive\03_Arbeit\02_HIWI Stellen\03_Architectural_Computing\TTL Files\repo-config.ttl");
            HttpContent fileStreamContent = new StreamContent(file);

            // Add Filetype even necessary?
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("Turtle/ttl");

            // Create new formData and Add the Config File to it as name config (http://localhost:7200/webapi)
            var formData = new MultipartFormDataContent();
            formData.Add(fileStreamContent, name: "config", fileName: "repo-config.ttl");



            // Post Respository Request
            var result = client.PostAsync(endpoint, formData).Result;
            var json = result.Content.ReadAsStringAsync().Result;

            // Check if security is enabled
            var resultSec = client.GetAsync("http://localhost:7200/rest/security").Result;
            var jsonSec = result.Content.ReadAsStringAsync().Result;
            
            // Post Data to Repository (also update data)
            //IGraph g = new Graph();
            String ttlBHoMFile = File.ReadAllText(@"G:\My Drive\03_Arbeit\02_HIWI Stellen\03_Architectural_Computing\TTL Files\BhomOuput.ttl");
            StringContent ttlFile = new StringContent(ttlBHoMFile);
            ttlFile.Headers.ContentType = new MediaTypeHeaderValue("text/turtle");


            var resultData = client.PutAsync("http://localhost:7200/repositories/BHoMVisualizationRepo04_08_2022/statements", ttlFile).Result;
            var jsonData = result.Content.ReadAsStringAsync().Result;

            // HTTP Delete Request with new endpoint (with {RepositoryID} at the end)
            var endpoint2 = new Uri("http://localhost:7200/rest/repositories/BHoMVisualizationRepo04_08_2022");
            var result2 = client.DeleteAsync(endpoint2).Result;
           var json2 = result.Content.ReadAsStringAsync().Result;

            
        }
    }
}
