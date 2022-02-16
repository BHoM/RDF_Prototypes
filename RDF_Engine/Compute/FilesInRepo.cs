using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static HashSet<string> FilesInRepo(string parentRepoDirectoryPath, string cacheRootDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(parentRepoDirectoryPath) || !Directory.Exists(parentRepoDirectoryPath))
                return null;

            // Read from cached memory.
            if (m_allCsFilePaths != null)
                return m_allCsFilePaths;

            string[] files = null;

            // Try to read from cached on disk.
            if (cacheRootDirectory == null)
                cacheRootDirectory = Directory.GetParent(Directory.GetParent(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).FullName).FullName;

            string cacheFilePath = null;
            if (!string.IsNullOrWhiteSpace(cacheRootDirectory))
                cacheFilePath = cacheRootDirectory + "\\cached_GithubRootCsFilepaths.txt";

            if (!string.IsNullOrWhiteSpace(cacheFilePath) && File.Exists(cacheFilePath))
            {
                files = File.ReadAllLines(cacheFilePath);
            }
            else
            {
                // Read the filesystem and get the .cs files.
                files = Directory.GetFiles(parentRepoDirectoryPath, "*.cs", SearchOption.AllDirectories);
                files = files.Where(f => 
                    !f.Contains("TemporaryGeneratedFile_") && 
                    !f.EndsWith("AssemblyInfo.cs") && 
                    !f.EndsWith("Resources.Designer.cs") && 
                    !f.EndsWith("AssemblyAttributes.cs") &&
                    !(f.Contains("packages") && !f.Contains("src")) // removes nuget package source files
                    ).ToArray();
            }

            // Cache the results to disk.
            if (files != null && !string.IsNullOrWhiteSpace(cacheFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath));
                File.WriteAllLines(cacheFilePath, files);
            }

            // Cache in memory.
            m_allCsFilePaths = new HashSet<string>(files);

            return m_allCsFilePaths;
        }

        // All ".cs" file paths found in the specified Root repository (e.g. C:/Users/alombardi/GitHub).
        private static HashSet<string> m_allCsFilePaths;
    }
}
