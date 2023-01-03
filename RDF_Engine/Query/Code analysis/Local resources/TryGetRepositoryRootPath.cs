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
        private static string m_repositoryRoot = null;

        [Description("Tries to get the root of git repositories from various common locations." +
            "A valid git root path is a directory that contains, among other repositories, also the BHoM repository.")]
        public static bool TryGetRepositoryRootPath(out string repositoryRoot)
        {
            if (!string.IsNullOrWhiteSpace(m_repositoryRoot))
            {
                repositoryRoot = m_repositoryRoot;
                return true;
            }

            repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            // Look in Documents
            repositoryRoot = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).ToString(), Environment.UserName, "Documents", "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            // Try removing special chars from username
            repositoryRoot = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).ToString(), Environment.UserName.OnlyAlphabetic(), "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            // Try removing special chars from username and look in Documents
            repositoryRoot = Path.Combine(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)).ToString(), Environment.UserName.OnlyAlphabetic(), "Documents", "GitHub");
            if (repositoryRoot.IsValidRepositoryRootPath())
                return true;

            return false;
        }
    }
}
