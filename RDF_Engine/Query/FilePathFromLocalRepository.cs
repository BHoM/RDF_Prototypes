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
        private static Dictionary<Type, string> m_cachedTypeFilePaths = new Dictionary<Type, string>();

        [Description("The method will look for a file named using standard BHoM filename convention for Types. " +
            "For example, for the type `BH.oM.Structure.Elements.Bar`, the method will look for 'Bar.cs', and the filepath will have to contain the 'namespaceGroup' called `Structure`.")]
        public static string FilePathFromLocalRepository(this Type type, string repositoryRoot, string cacheRootDirectory = null, bool relative = false)
        {
            if (string.IsNullOrWhiteSpace(repositoryRoot) || !Directory.Exists(repositoryRoot))
            {
                repositoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "GitHub");
                if (!Directory.Exists(repositoryRoot))
                {
                    log.RecordError($"Could not find Local repository directory on disk at path: {repositoryRoot}", true);
                    return null;
                }
            }

            string filepath = null;

            // Check the cached types first.
            if (m_cachedTypeFilePaths?.TryGetValue(type, out filepath) ?? false)
                return relative ? filepath?.Replace(repositoryRoot, "") : filepath;

            HashSet<string> allFilePaths = Compute.FilesInRepo(repositoryRoot, cacheRootDirectory);

            string nameSpaceGroup = type.Namespace.Split('.')[2]; // [2] selects anything exactly after `BH.oM.` or `BH.Engine.`

            if (nameSpaceGroup == "Adapters" || nameSpaceGroup == "External")
                nameSpaceGroup = type.Namespace.Split('.')[3];

            if (nameSpaceGroup == "Base")
                nameSpaceGroup = "BHoM";
            else
                nameSpaceGroup += "_oM";

            List<string> matchingFilePaths = allFilePaths?.Where(p =>
                Path.GetDirectoryName(p).Contains($"{nameSpaceGroup}") &&
                !Path.GetDirectoryName(p).Contains("Engine") &&
                Path.GetFileNameWithoutExtension(p) == type.NameValidChars()).ToList();

            if (matchingFilePaths.Count() > 1)
                log.RecordWarning($"Found more than one matching filepath for `{type.FullName}`: {string.Join(", ", matchingFilePaths)}", true);
            else
            {
                filepath = matchingFilePaths.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(filepath))
                    log.RecordWarning($"Could not find filepath for Type `{type.FullName}`", true);
            }

            // Store in cache. If no filePath was found, stores null string, which is better than having to reperform the search.
            m_cachedTypeFilePaths[type] = filepath;

            return relative ? filepath?.Replace(repositoryRoot, "") : filepath; // if not found, this returns null.
        }
    }
}
