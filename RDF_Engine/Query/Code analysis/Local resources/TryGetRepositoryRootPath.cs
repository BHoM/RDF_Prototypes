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
        private static HashSet<string> validPathsFound = new HashSet<string>();

        [Description("Tries to get the root of git repositories from various common locations." +
            "A valid git root path is a directory that contains, among other repositories, also the BHoM repository.")]
        public static bool TryGetRepositoryRootPath(out string repositoryRoot)
        {
            repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");

            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");

            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            return false;
        }
    }
}
