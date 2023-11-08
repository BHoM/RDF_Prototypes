using System.IO;

namespace BH.UI.Engine.GraphDB
{
    public class LoginDataRetriever
    {
        public string RetrievePassword(string serverAddress, string username)
        {

            SecureStorage secureStorage = new SecureStorage();

            // Check if combination of server and username is already stored, if so return password for API interactions
            string jsonFilePath = ConstructJSONPath(serverAddress, username);
            if (File.Exists(jsonFilePath))
                return secureStorage.GetCredentials(jsonFilePath, username);

            // if not return null and login is called after 
            return null;
        }

        public static string MakeValidFileName(string name, string username)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name + " " + username, invalidRegStr, "_");
        }

        public static string ConstructJSONPath(string serverAddress, string username)
        {
            string potentialJsonFile = $"{MakeValidFileName(serverAddress, username)}.json";
            return Path.Combine(Path.GetTempPath(), potentialJsonFile); //C:\Users\User\AppData\Local\Temp + Filename
        }

    }

}


