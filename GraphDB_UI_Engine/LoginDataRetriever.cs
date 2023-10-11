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
        public string PopUpBrowser(string serverAdress, string username) 
        {

            // check for JSON file (name like server address) if login credentials are already there for URI and username input to the methode
            // if found return cred.
            // if not found -> popup window
            // -> store credentials
            // return cred. 

            string potentialJsonFile = $"{MakeValidFileName(serverAdress)}.json";
            string jsonFilePath = Path.Combine(Path.GetTempPath(), potentialJsonFile); //C:\Users\User\AppData\Local\Temp + Filename

            SecureStorage secureStorage = new SecureStorage();

            // -------------------------------- UNCOMMENT AFTER DEBUGGING -----------------------------------
            //if (File.Exists(jsonFilePath))
            //    return secureStorage.GetCredentials(jsonFilePath, username);

            // Navigate to the local HTML file (interface.html).
            string url = new Uri(Path.GetFullPath(@"C:\Users\Aaron\Documents\GitHub\RDF_Prototypes\GraphDB_UI_Engine\interface.html")).AbsoluteUri; //replace with generic path from BHOM Assemblies
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--app=" + url);
            IWebDriver driver = null;


            try
            {
                new DriverManager().SetUpDriver(new ChromeConfig());
                driver = new ChromeDriver(options);

            }
            catch (Exception e )
            {
                if (e is InvalidOperationException || e is UnauthorizedAccessException)
                {                
                // Update the chromedriver, then retry.
                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
                driver = new ChromeDriver(options);
                driver.Manage().Window.Size = new System.Drawing.Size(500, 450);
                }
            }


            if (driver == null)
                return null;

            


            // Add a wait for the alert
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d =>
            {
                try
                {
                    d.SwitchTo().Alert();
                    return true;
                }
                catch (NoAlertPresentException)
                {
                    return false;
                }
            });

            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;
            alert.Accept();

            // TO-DO finish with json and than switch to credentialcache put into (different function)
            // 

            // Parse JSON and save it into the file
            dynamic data = JsonConvert.DeserializeObject(alertText);

            string tempPath = Path.GetTempPath();
            string jsonFile = $"{MakeValidFileName(serverAdress)}.json";
            string filePath = Path.Combine(tempPath, jsonFile);

            string mystring = JsonConvert.SerializeObject(data, Formatting.Indented); // somehow the deserialized alert is in a weird format

            dynamic parsedJson = JsonConvert.DeserializeObject(mystring);

            string user = parsedJson.username;
            string pw = parsedJson.password;

            secureStorage.SaveCredentials(user, pw, filePath);

            driver.Close();

            return secureStorage.GetCredentials(filePath, username);
        }

        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

    }

}


