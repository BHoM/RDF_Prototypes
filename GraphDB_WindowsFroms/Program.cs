using System.Text;
using BH.UI.Engine.GraphDB;
using Newtonsoft.Json;

namespace GraphDB_WindowsForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string serverAddress = null;

            if (args.Length > 0)
            {
                serverAddress = args[0];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm loginForm = new LoginForm();


            // Runs after loginform successfully closes
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                var credentials = new
                {
                    username = loginForm.Username,
                password = loginForm.Password
                };

                if (string.IsNullOrEmpty(serverAddress))
                {
                    serverAddress = loginForm.ServerAddress;
                }

                // TO-DO: API call here for authentification token -> if not successfull start again -> if successfull, store token in Adapter?
                var client = new HttpClient();

                // Set the base address for HTTP requests
                client.BaseAddress = new Uri(serverAddress); //TO-DO throw exception if url is not valid

                // Serialize the credentials object to JSON
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Send a POST request to the login endpoint
                var response =  client.PostAsync("rest/login", content);
                
                if (!response.Result.IsSuccessStatusCode)
                {
                    var statusCode = response.Result.StatusCode; // might not have a statusCode depending if there is an exception like endpoint doesnt exist
                    // open login again and display status code from request ( e.g 401 invalid credentials)
                }

                // Retrieve the authentication header from the response
                if (response.Result.Headers.TryGetValues("authorization", out var authHeaders))
                {
                    string authToken = string.Join("Authorization: ", authHeaders);
                    // Return gdbToken and use like ('Authorization: GDB eyJ1c2VybmFtZSI6ImFkbWlu...')
                }
            }
        }
    }
}