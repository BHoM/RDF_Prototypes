using Newtonsoft.Json;
using System.Text;
using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;


namespace BH.UI.Engine.GraphDB
{
    public class LoginDataRetriever
    {
        public string PopUpBrowser() // 1 argument server adress (store json file for each serveraddress) and 2. argument username RETURN credentials
        {

            // check for JSON file (name like server address) if login credentials are already there for URI and username input to the methode
            // if found return cred.
            // if not found -> popup window
            // -> store credentials
            // return cred.


            // Create a ChromeDriver instance.
            IWebDriver driver = new ChromeDriver("C:/path/to/chromedriver.exe");

            // Navigate to the local HTML file (interface.html).
            string url = new Uri(Path.GetFullPath(@"C:\Users\Aaron\Documents\GitHub\RDF_Prototypes\GraphDB_UI_Engine\interface.html")).AbsoluteUri; //replace with generic path
            driver.Navigate().GoToUrl(url);


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

            // Wait for some time to let console log to be generated
            System.Threading.Thread.Sleep(5000);

            // TO-DO finish with json and than switch to credentialcache put into (different function)
            // 

            // Parse JSON and save it into the file
            var logs = driver.Manage().Logs.GetLog(LogType.Browser);
            foreach (var log in logs)
            {
                if (log.Message.Contains("{") && log.Message.Contains("}"))
                {
                    var jsonString = log.Message.Substring(log.Message.IndexOf('{'),
                        log.Message.LastIndexOf('}') - log.Message.IndexOf('{') + 1);
                    dynamic data = JsonConvert.DeserializeObject(jsonString);
                    string tempPath = Path.GetTempPath();
                    string filePath = Path.Combine(tempPath, "loginData.json");
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Formatting.Indented), Encoding.UTF8);
                    break;
                }
            }

            driver.Close();

            // return password but not directly
        }

    }
}


