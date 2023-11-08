using System;
using System.Text;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace BH.UI.Engine.GraphDB
{
    public class LoginDataRetriever
    {
        public string RetrievePassword(string serverAddress, string username)
        {

            string potentialJsonFile = $"{MakeValidFileName(serverAddress)}.json";
            string jsonFilePath = Path.Combine(Path.GetTempPath(), potentialJsonFile); //C:\Users\User\AppData\Local\Temp + Filename

            SecureStorage secureStorage = new SecureStorage();

            // Check if combination of server and username is already stored, if so return password for API interactions
            if (File.Exists(jsonFilePath))
                return secureStorage.GetCredentials(jsonFilePath, username);

            // if not return null and login is called after 
            return null;
        }

        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

    }

}


