using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using BH.Engine.RDF;
using BH.oM.RDF;
using BH.Test.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Update;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static partial class Compute
    {

        public static void StartGraphDB()
        {
            Process.Start(@"C:\Users\Aaron\AppData\Local\GraphDB Desktop\GraphDB Desktop.exe");
            //Process.Start("GraphDB Desktop.exe");
            if (Process.GetProcessesByName("GraphDB Desktop").Any())
                    {
                        // API Call
                    }
        }
        public static void PostToRepo(string filePathRepoConfig, string filePathRDFData, string serverAddress = "http://localhost:7200/", string repoConfigFile = "repo-config.ttl", string repoName = "BHoMVisualization")
        {
            // Documentation: http://localhost:7200/webapi

            // Create Http Client and first Endpoint
            var client = new HttpClient();
            var endpointRepoCreate = new Uri(serverAddress + "rest/repositories/");

            // Get repository config file and turn into HTTP Content
            FileStream file = File.OpenRead(filePathRepoConfig);
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

            var endpointRepoPostData = new Uri(serverAddress + "repositories/" + repoName + "/statements");
            var resultData = client.PutAsync(endpointRepoPostData, ttlFile).Result;
            var jsonData = result.Content.ReadAsStringAsync().Result;

            // HTTP Delete Request with new endpoint (with {RepositoryID} at the end)
            var endpoint2 = new Uri("http://localhost:7200/rest/repositories/BHoMVisualization");
            var result2 = client.DeleteAsync(endpoint2).Result;
            var json2 = result.Content.ReadAsStringAsync().Result;

        }
    }
    public static class Program
    {
        public static void Main(string[] args )
        {
            Compute.PostToRepo(@"G:\My Drive\03_Arbeit\02_HIWI Stellen\03_Architectural_Computing\TTL Files\repo-config.ttl", @"G:\My Drive\03_Arbeit\02_HIWI Stellen\03_Architectural_Computing\TTL Files\BhomOuput.ttl");
            //Compute.StartGraphDB();

        }
    }
}
