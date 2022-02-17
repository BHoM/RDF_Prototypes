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
        public static Uri GithubURIFromNamespace(this Type t)
        {
            if (t.Name.StartsWith("<>c__"))
                return null;

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
            if (!Uri.TryCreate(result, UriKind.Absolute, out uri))
                log.RecordError($"Could not compose a valid URL for type {t.FullName}", true);

            return uri;
        }
    }
}