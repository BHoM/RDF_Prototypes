
using BH.Engine.RDF.Types;
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
        public static Uri GithubURI(this Type typeToSearch, LocalRepositorySettings repoSettings)
        {
            // Null guards
            if (typeToSearch == null || !typeToSearch.IsBHoMType())
                return null;

            repoSettings = repoSettings ?? new LocalRepositorySettings();

            // Custom types exception.
            if (typeToSearch is CustomObjectType)
                return ((CustomObjectType)typeToSearch).OntologicalUri;

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(typeToSearch, repoSettings);
            if (result == null)
            {
                // If the previous fails, compose a GitHub uri by leveraging namespace/folder conventions. This will fail wherever conventions are not respected.
                result = GithubURIFromNamespace(typeToSearch);
            }

            return result;
        }

        public static Uri GithubURI(this MemberInfo miToSearch, LocalRepositorySettings repoSettings)
        {
            // Null guards
            if (miToSearch == null)
                return null;

            repoSettings = repoSettings ?? new LocalRepositorySettings();

            // Custom types exception.
            if (miToSearch is CustomPropertyInfo)
                return ((CustomPropertyInfo)miToSearch).OntologicalUri;

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(miToSearch as dynamic, repoSettings);

            return result;
        }
    }
}
