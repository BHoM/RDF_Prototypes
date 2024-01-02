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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace BH.Engine.Adapters.GraphDB
{
    public static partial class Compute
    {

        public static string FindExecutable(string folderNameToSearch = "GraphDB")
        {
            string exeFileExtension = ".exe";

            // Get the path to the LocalAppData folder for the current user
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Get all subdirectories in the LocalAppData folder
            string[] subdirectories = Directory.GetDirectories(localAppDataPath);

            // Search for the target folder
            string targetFolderPath = null;
            foreach (string subdirectory in subdirectories)
            {
                if (Path.GetFileName(subdirectory).IndexOf(folderNameToSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine($"Found the target folder: {subdirectory}");
                    targetFolderPath = subdirectory;
                    break;
                }
            }

            if (targetFolderPath != null)
            {
                // Get all files in the target folder
                string[] files = Directory.GetFiles(targetFolderPath);

                // Search for the exe file in the target folder
                bool exeFound = false;
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).Equals(exeFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Found exe file: {file}");
                        return file;
                    }
                }

                if (!exeFound)
                {
                    Console.WriteLine($"No exe files found in the folder '{folderNameToSearch}'.");
                }
            }
            else
            {
                Console.WriteLine($"Folder with the name '{folderNameToSearch}' not found.");
            }
            return null;

        }
    }
}
