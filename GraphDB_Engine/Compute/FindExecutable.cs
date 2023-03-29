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
