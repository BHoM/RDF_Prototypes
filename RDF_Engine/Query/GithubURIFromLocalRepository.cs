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
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        public static Uri GithubURIFromLocalRepository(this Type typeToSearch, TBoxSettings settings)
        {
            if (typeToSearch.Name.StartsWith("<>c__"))
                return null;

            string typeFilePath = typeToSearch.FilePathFromLocalRepository(settings, true);

            if (!string.IsNullOrWhiteSpace(typeFilePath))
            {
                var pathComponents = typeFilePath.Split(Path.DirectorySeparatorChar).Where(pc => !string.IsNullOrWhiteSpace(pc)).ToList();

                try
                {
                    Uri URL = CombineUris(settings.GithubOrganisation, pathComponents[0], "/blob/main/", string.Join("/", pathComponents.Skip(1)));
                    return URL;
                }
                catch { }
            }

            return null;
        }

        public static Uri GithubURIFromLocalRepository(this MemberInfo pi, TBoxSettings settings)
        {
            Uri declaringTypeUri = pi.DeclaringType.GithubURIFromLocalRepository(settings);

            int lineNumber = Compute.LineNumber(pi as dynamic, settings);

            if (lineNumber < 0)
                return declaringTypeUri;

            Uri result = new Uri($"{ declaringTypeUri.ToString()}#L{lineNumber + 1}"); // need to add +1 because Github Line numbers start from 1.

            return result;
        }
    }
}