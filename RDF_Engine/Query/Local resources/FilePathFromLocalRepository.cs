using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.RDF;
using System;
using System.Collections;
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
        private static Dictionary<Type, string> m_cachedTypeFilePaths = new Dictionary<Type, string>();

        [Description("The method will look for a file named using standard BHoM filename convention for Types. " +
            "For example, for the type `BH.oM.Structure.Elements.Bar`, the method will look for 'Bar.cs', and the filepath will have to contain the 'namespaceGroup' called `Structure`.")]
        public static string FilePathFromLocalRepository(this Type type, LocalRepositorySettings settings, bool getRelativePath = false)
        {
            if (settings == null)
                settings = new LocalRepositorySettings();

            string repositoryRoot = settings.RepositoryRootPath;

            if (string.IsNullOrWhiteSpace(repositoryRoot) || !Directory.Exists(repositoryRoot))
            {
                Log.RecordError($"Could not find Local repository directory on disk at path: {repositoryRoot}", true);
                return null;
            }

            string filepath = null;

            // Check the cached types first.
            if ((m_cachedTypeFilePaths?.TryGetValue(type, out filepath) ?? false) && !filepath.IsNullOrEmpty())
                return getRelativePath ? filepath?.Replace(repositoryRoot, "") : filepath;

            string typeNameValidChars = type.NameValidChars();
            HashSet<string> allFilePaths = Compute.FilesInRepo(repositoryRoot, settings);

            string nameSpaceGroup = type.Namespace.Split('.')[2]; // [2] selects anything exactly after `BH.oM.` or `BH.Engine.`

            if (nameSpaceGroup == "Adapters" || nameSpaceGroup == "External")
                nameSpaceGroup = type.Namespace.Split('.')[3];

            if (nameSpaceGroup == "Base")
                nameSpaceGroup = "BHoM";
            else
                nameSpaceGroup += "_oM";

            List<string> matchingFilePaths = allFilePaths?.Where(p =>
                Path.GetDirectoryName(p).Contains($"{nameSpaceGroup}") &&
                !Path.GetDirectoryName(p).Contains("Engine") && // believe it or not, it's useful to remove certain exceptions.
                Path.GetFileNameWithoutExtension(p) == typeNameValidChars).ToList();

            if (matchingFilePaths.Count() > 1)
            {
                Log.RecordWarning($"Found more than one matching filepath for `{type.FullName}`: {string.Join(", ", matchingFilePaths)}", true);

                // Store null in cache, which is better than having to reperform the search.
                m_cachedTypeFilePaths[type] = null;

                return null;
            }

            // Try removing the namespaceGroup condition and see if we can find at least one match. Can help for types e.g. Structure_oM hosted in `Structural` folders (e.g. StructuralEngineering_Toolkit).
            if (!matchingFilePaths.Any())
            {
                matchingFilePaths = allFilePaths?.Where(p =>
                    //Path.GetDirectoryName(p).Contains($"_oM") &&
                    !Path.GetDirectoryName(p).Contains("_Engine") &&  // believe it or not, it's useful to remove certain exceptions.
                    Path.GetFileNameWithoutExtension(p) == typeNameValidChars).ToList();
            }

            // Try checking if filename *contains* the type name. Helps for files with non compliant prefixes/suffixes (e.g. _IExecuteCommand.cs in BHoM_Adapter).
            if (!matchingFilePaths.Any())
            {
                matchingFilePaths = allFilePaths?.Where(p =>
                    //Path.GetDirectoryName(p).Contains($"_oM") &&
                    !Path.GetDirectoryName(p).Contains("_Engine") &&  // believe it or not, it's useful to remove certain exceptions.
                    Path.GetFileNameWithoutExtension(p).Contains(typeNameValidChars)).ToList();
            }

            // Try lowercasing
            if (!matchingFilePaths.Any())
            {
                // Try removing the namespaceGroup condition and see if we can find at least one match. Can help for types e.g. Structure_oM hosted in `Structural` folders (e.g. StructuralEngineering_Toolkit).
                matchingFilePaths = allFilePaths?.Where(p =>
                    //Path.GetDirectoryName(p).Contains($"_oM") &&
                    !Path.GetDirectoryName(p).Contains("_Engine") &&  // believe it or not, it's useful to remove certain exceptions.
                    Path.GetFileNameWithoutExtension(p).ToLower() == typeNameValidChars.ToLower()).ToList();
            }

            if (matchingFilePaths.Count != 1)
                Log.RecordWarning($"Could not find filepath for Type `{typeNameValidChars}`", true);

            filepath = matchingFilePaths.FirstOrDefault();

            // Store in cache.
            if (!filepath.IsNullOrEmpty())
                m_cachedTypeFilePaths[type] = filepath;
            else
            {
                Log.RecordWarning($"Could not compute repository filepath for type {type.FullName}.");
            }

            return getRelativePath ? filepath?.Replace(repositoryRoot, "") : filepath; // if not found, this returns null.
        }
    }
}
