using Newtonsoft.Json;
using System.Text;
using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


namespace BH.Engine.Adapters.GraphDBUI
{
    public partial class LoginDataRetriever
    {
        public void RetrieveLoginData()
        {
            // Set the path to the ChromeDriver executable.
            string driverPath = @"C:\Users\%USERNAME%\AppData\Local\Google\Chrome\Application\chrome.exe"; 

            // Create a ChromeDriver instance.
            IWebDriver driver = new ChromeDriver(driverPath);

            // Navigate to the local HTML file (interface.html).
            string url = new Uri(Path.GetFullPath(@"C:\Users\Aaron\Documents\GitHub\RDF_Prototypes\GraphDBUI\interface.html")).AbsoluteUri; //replace with generic path
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
        }

    }
}

