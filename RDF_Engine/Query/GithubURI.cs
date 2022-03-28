
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
        public static Uri GithubURI(this Type typeToSearch, TBoxSettings settings)
        {
            if (!typeToSearch.IsBHoMType())
                return null;

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(typeToSearch, settings);
            if (result == null)
            {
                // If the previous fails, compose a GitHub uri by leveraging namespace/folder conventions. This will fail wherever conventions are not respected.
                result = GithubURIFromNamespace(typeToSearch);
            }

            return result;
        }

        public static Uri GithubURI(this MemberInfo typeToSearch, TBoxSettings settings)
        {
            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(typeToSearch as dynamic, settings);

            return result;
        }
    }
}
