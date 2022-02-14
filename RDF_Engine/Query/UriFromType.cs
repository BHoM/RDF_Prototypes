
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

        public static Uri GithubURI(this Type typeToSearch, string githubRootDirectory = null, Uri githubOrganisation = null, string cacheRootDirectory = null)
        {
            if (githubOrganisation == null)
                githubOrganisation = m_BHoMGithubOrganization;

            if (cacheRootDirectory == null)
                cacheRootDirectory = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

            Uri result = GithubURIFromFileSystem(typeToSearch, githubRootDirectory, githubOrganisation, cacheRootDirectory);
            if (result == null)
                result = ComposedGithubURI(typeToSearch);

            return result;
        }

        public static Uri ComposedGithubURI(this Type t)
        {
            string result = null;

            Assembly assembly = Assembly.GetAssembly(t);
            string toolkitName = null;

            // Assuming everything is in this BHoM org
            string baseUri = @"https://github.com/BHoM/";

            if (assembly.IsToolkitAssembly(out toolkitName))
            {
                List<string> fullPathSplit = t.FullName.Replace("BH.oM", "").Split('.').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                fullPathSplit[0] = fullPathSplit[0] + "_oM";
                fullPathSplit.RemoveAt(fullPathSplit.Count - 1);

                string relativeUri = @"BHoM/blob/main/"
                    + string.Join("/", fullPathSplit)
                    + $"/{t.NameValidChars()}.cs";

                result = baseUri + relativeUri;
            }
            else
            {
                List<string> fullPathSplit = t.FullName.Replace("BH.oM.Adapters", "").Replace("BH.oM", "").Split('.').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                fullPathSplit[0] = fullPathSplit[0] + "_oM";
                fullPathSplit.RemoveAt(fullPathSplit.Count - 1);

                string relativeUri = toolkitName + @"BHoM/blob/main/"
                    + string.Join("/", fullPathSplit)
                    + $"/{t.NameValidChars()}.cs";

                #region Corrections for non-compliant file paths
                // Corrections for non-compliant file paths
                if (relativeUri.Contains("Base_oM") && t.IsInterface)
                    relativeUri = toolkitName + @"BHoM/blob/main/"
                    + string.Join("/", fullPathSplit)
                    + $"/Interface"
                    + $"/{t.NameValidChars()}.cs";

                relativeUri = relativeUri.Replace("Base_oM", "BHoM");

                #endregion

                result = baseUri + relativeUri;
            }

            Uri uri = null;
            if (Uri.TryCreate(result, UriKind.Absolute, out uri))
                log.RecordError($"Could not compose a valid URL for type {t.FullName}", true);

            return uri;
        }

        public static Uri GithubURIFromFileSystem(this Type typeToSearch, string githubRootDirectory = null, Uri githubOrganisation = null, string cacheRootDirectory = null)
        {
            if (githubOrganisation == null)
                githubOrganisation = m_BHoMGithubOrganization;

            if (cacheRootDirectory == null)
                cacheRootDirectory = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;

            if (string.IsNullOrWhiteSpace(githubRootDirectory) || !Directory.Exists(githubRootDirectory))
            {
                githubRootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");
                if (!Directory.Exists(githubRootDirectory))
                {
                    log.RecordError($"Could not find Github directory on disk at path: {githubRootDirectory}", true);
                    return null;
                }
            }

            string typeFilePath = typeToSearch.TypeFilePath(githubRootDirectory, cacheRootDirectory)?.Replace(githubRootDirectory, "");

            if (!string.IsNullOrWhiteSpace(typeFilePath))
            {
                var pathComponents = typeFilePath.Split(Path.DirectorySeparatorChar).Where(pc => !string.IsNullOrWhiteSpace(pc)).ToList();

                try
                {
                    Uri URL = CombineUris(githubOrganisation, pathComponents[0], "/blob/main/", string.Join("/", pathComponents.Skip(1)));
                    return URL;
                }
                catch { }
            }

            return null;
        }


        [Input("typeToSearch", "The method will look for a file named using standard BHoM filename convention for Types. " +
            "For example, for the type `BH.oM.Structure.Elements.Bar`, the method will look for 'Bar.cs'.")]
        public static string TypeFilePath(this Type typeToSearch, string githubRootDirectory, string cacheRootDirectory = null)
        {
            string typeFileToSearch = typeToSearch.NameValidChars() + ".cs";

            string filepath = Compute.FilesInRepo(githubRootDirectory, cacheRootDirectory).FirstOrDefault(path => path.EndsWith(typeFileToSearch));
            if (string.IsNullOrWhiteSpace(filepath))
                log.RecordWarning($"Could not find filepath for Type `{typeToSearch.FullName}`", true);

            return filepath; // If not found, this is null.
        }
    }
}
