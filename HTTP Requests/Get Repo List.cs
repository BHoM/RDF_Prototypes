using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Requests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // HTTP Get Request for a List of Repositories
            using (var client = new HttpClient())  
            {
                var endpoint = new Uri("http://localhost:7200/rest/repositories/");
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;

            }
        }
    }
}
