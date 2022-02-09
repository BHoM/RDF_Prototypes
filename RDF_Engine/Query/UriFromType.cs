
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static string UriFromType(this Type t)
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

                // Corrections for out-of-convention file paths
                if (relativeUri.Contains("Base_oM") && t.IsInterface)
                    relativeUri = toolkitName + @"BHoM/blob/main/"
                    + string.Join("/", fullPathSplit)
                    + $"/Interface"
                    + $"/{t.NameValidChars()}.cs";

                relativeUri = relativeUri.Replace("Base_oM", "BHoM");

                result = baseUri + relativeUri;
            }

            return result.ToString();
        }
    }
}
