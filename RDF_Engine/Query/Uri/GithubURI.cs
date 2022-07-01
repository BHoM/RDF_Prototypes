
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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the Uri to be used for the Ontology of the input Type. " +
            "If the input Type is a BHoM type, the Uri will be calculated from the file structure of a local copy of the main BHoM repository, as instructed by the `LocalRepositorySettings` input." +
            "Other Uri options can be found in the `TBoxSettings` input.")]
        public static Uri OntologyUri(this Type type, TBoxSettings tBoxSettings, LocalRepositorySettings repoSettings)
        {
            tBoxSettings = tBoxSettings ?? new TBoxSettings();

            // See if there is a particular URI specified for this type.
            string customUriString = null;
            Uri customUri = null;

            if (tBoxSettings.TypeUris.TryGetValue(type, out customUriString))
                if (System.Uri.TryCreate(customUriString, UriKind.Absolute, out customUri))
                    return customUri;
                else
                    Log.RecordWarning($"URI `{customUriString}` specified for type {type.FullName} is invalid.");

            if (type.IsBHoMType())
                return type.GithubURI(repoSettings);

            // Use default base Uri for unknown types.
            Uri defaultUri = null;
            if (System.Uri.TryCreate(tBoxSettings.DefaultBaseUriForUnknownTypes + type.Name, UriKind.Absolute, out defaultUri))
                return customUri;
            else
                Log.RecordWarning($"Default base URI for `{tBoxSettings.DefaultBaseUriForUnknownTypes}` specified for unknown types is invalid.");

            return new Uri(new TBoxSettings().DefaultBaseUriForUnknownTypes + $"#{type.Name}");
        }

        /***************************************************/

        [Description("Returns the Uri to be used for the Ontology of the input MemberInfo. " +
            "If the input Type is a BHoM type, the Uri will be calculated from the file structure of a local copy of the main BHoM repository, as instructed by the `LocalRepositorySettings` input." +
            "Other Uri options can be found in the `TBoxSettings` input.")]
        public static Uri OntologyURI(this MemberInfo miToSearch, TBoxSettings tBoxSettings, LocalRepositorySettings repoSettings)
        {
            // Null guards
            if (miToSearch == null)
                return null;

            repoSettings = repoSettings ?? new LocalRepositorySettings();

            // Custom property exception.
            if (miToSearch is CustomPropertyInfo)
                return Query.CombineUris(miToSearch.DeclaringType.OntologyURI(tBoxSettings, repoSettings) + $"#{miToSearch.Name}");

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(miToSearch as dynamic, repoSettings);

            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /// <summary>
        /// Returns the URI of a BHoM type by calculating it from the file structure of the main BHoM repository. 
        /// This method must remain private and should never be used outside of the OntologyUri() method. Always use OntologyUri.
        /// </summary>
        private static Uri GithubURI(this Type typeToSearch, LocalRepositorySettings repoSettings)
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
    }
}
