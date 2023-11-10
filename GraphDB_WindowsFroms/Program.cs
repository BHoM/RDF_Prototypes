using BH.UI.Engine.GraphDB;

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
                string username = loginForm.Username;
                string password = loginForm.Password;
                if (string.IsNullOrEmpty(serverAddress))
                {
                    serverAddress = loginForm.ServerAddress;
                }


                SecureStorage secureStorage = new SecureStorage();
                string tempPath = Path.GetTempPath();
                string jsonFile = $"{MakeValidFileName(serverAddress, username)}.json";
                string filePath = Path.Combine(tempPath, jsonFile);

                secureStorage.SaveCredentials(username, password, filePath);
            }
        }




        public static string MakeValidFileName(string name, string username)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name + " " + username, invalidRegStr, "_");
        }

    }


}