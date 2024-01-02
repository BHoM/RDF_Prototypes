/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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