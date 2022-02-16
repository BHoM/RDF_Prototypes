
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Uri GithubURI(this Type typeToSearch, string githubRootDirectory = null, Uri githubOrganisation = null, string cacheRootDirectory = null)
        {
            if (typeToSearch.Name.StartsWith("<>c__"))
                return null;


            if (cacheRootDirectory == null)
                cacheRootDirectory = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(typeToSearch, githubRootDirectory, githubOrganisation, cacheRootDirectory);
            if (result == null)
            {
                // If the previous fails, compose a GitHub uri by leveraging namespace/folder conventions. This will fail wherever conventions are not respected.
                result = GithubURIFromNamespace(typeToSearch);
            }

            return result;
        }

        public static Uri GithubURI(this MemberInfo typeToSearch, string githubRootDirectory = null, Uri githubOrganisation = null, string cacheRootDirectory = null)
        {
            if (typeToSearch.Name.StartsWith("<>c__"))
                return null;

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(typeToSearch as dynamic, githubRootDirectory, githubOrganisation, cacheRootDirectory);

            return result;
        }
    }
}
