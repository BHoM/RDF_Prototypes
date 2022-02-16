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
        private static Uri m_BHoMGithubOrganization = new Uri(@"https://github.com/BHoM/");

        public static Uri GithubURIFromLocalRepository(this Type typeToSearch, string githubRootDirectory = null, Uri githubOrganisation = null, string cacheRootDirectory = null)
        {
            if (typeToSearch.Name.StartsWith("<>c__"))
                return null;

            string typeFilePath = typeToSearch.FilePathFromLocalRepository(githubRootDirectory, cacheRootDirectory, true);

            if (!string.IsNullOrWhiteSpace(typeFilePath))
            {
                var pathComponents = typeFilePath.Split(Path.DirectorySeparatorChar).Where(pc => !string.IsNullOrWhiteSpace(pc)).ToList();

                if (githubOrganisation == null)
                    githubOrganisation = m_BHoMGithubOrganization;

                try
                {
                    Uri URL = CombineUris(githubOrganisation, pathComponents[0], "/blob/main/", string.Join("/", pathComponents.Skip(1)));
                    return URL;
                }
                catch { }
            }

            return null;
        }

        public static Uri GithubURIFromLocalRepository(this MemberInfo pi, string githubRootDirectory = null, Uri githubOrganisation = null, string cacheRootDirectory = null)
        {
            Uri declaringTypeUri = pi.DeclaringType.GithubURIFromLocalRepository(githubRootDirectory, githubOrganisation, cacheRootDirectory);

            int lineNumber = Compute.LineNumber(pi as dynamic, githubRootDirectory, cacheRootDirectory);

            if (lineNumber < 0)
                return declaringTypeUri;

            Uri result = new Uri($"{ declaringTypeUri.ToString()}#L{lineNumber}");

            return result;
        }
    }
}