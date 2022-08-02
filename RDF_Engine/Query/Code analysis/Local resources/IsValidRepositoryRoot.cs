using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        private static HashSet<string> m_validRepoRootPathsFound = new HashSet<string>();

        [Description("Checks whether the input path points to a Git root path containing a BHoM repository clone." +
            "A valid 'repository root path' points to a directory that contains, among other repositories, also the BHoM repository.")]
        public static bool IsValidRepositoryRootPath(this string repositoryRootPath)
        {
            if (string.IsNullOrWhiteSpace(repositoryRootPath) || !Directory.Exists(repositoryRootPath))
                return false;

            if (m_validRepoRootPathsFound.Contains(repositoryRootPath))
                return true;

            repositoryRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");

            var allRepoDirectories = Directory.GetDirectories(repositoryRootPath);

            var bhomRepoDirectory = allRepoDirectories.Where(d => d.EndsWith("BHoM")).FirstOrDefault();
            if (Directory.GetFiles(bhomRepoDirectory).Where(f => f.EndsWith("BHoM.sln")).Count() == 1)
            {
                m_validRepoRootPathsFound.Add(repositoryRootPath);
                return true;
            }

            return false;
        }
    }
}
