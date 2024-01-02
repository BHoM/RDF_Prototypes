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


