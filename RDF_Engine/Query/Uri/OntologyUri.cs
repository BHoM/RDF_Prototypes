/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */


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

            if (tBoxSettings.TypeUris?.TryGetValue(type, out customUriString) ?? false)
                if (System.Uri.TryCreate(customUriString, UriKind.Absolute, out customUri))
                    return customUri;
                else
                    Log.RecordWarning($"URI `{customUriString}` specified for type {type.FullName} is invalid.");

            if (type.IsBHoMType())
            {
                // Uri for CustomType
                if (type is ICustomRDFType)
                    return Query.CombineUris(tBoxSettings.CustomObjectTypesBaseAddress, type.NameValidChars());

                Uri githubUri = type.GithubURI(tBoxSettings, repoSettings);
                if (githubUri != null)
                    return githubUri;
            }

            // Use default base Uri for unknown types.
            if (System.Uri.TryCreate(tBoxSettings.DefaultBaseUriForUnknownTypes, UriKind.Absolute, out _))
                return CombineUris(tBoxSettings.DefaultBaseUriForUnknownTypes, type.NameValidChars());
            else
                Log.RecordWarning($"Default base URI for `{tBoxSettings.DefaultBaseUriForUnknownTypes}` specified for unknown types is invalid.");

            return new Uri(new TBoxSettings().DefaultBaseUriForUnknownTypes + $"#{type.NameValidChars()}");
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

            if (miToSearch is Type)
                return OntologyUri((Type)miToSearch, tBoxSettings, repoSettings);

            // Custom property exception.
            if (miToSearch is CustomPropertyInfo)
            {
                if (miToSearch.DeclaringType.IsBHoMType() && !(miToSearch.DeclaringType is ICustomRDFType))
                {
                    // The CustomPropertyInfo is attached to a standard BHoM object that is not a CustomType.
                    // This means that this property is actually an entry in the CustomData dictionary of the object.
                    // TODO: See how to deal with this scenario better. For now, returning the BHoM object Uri + #propertyName.

                    return RDF.Query.CombineUris((string)(RDF.Query.OntologyUri(miToSearch.DeclaringType, tBoxSettings, repoSettings) + $"#{miToSearch.Name}"));
                }

                return RDF.Query.CombineUris((string)(RDF.Query.OntologyURI(miToSearch.DeclaringType, tBoxSettings, repoSettings) + $"#{miToSearch.Name}"));
            }

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(miToSearch as dynamic, tBoxSettings, repoSettings);

            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /// <summary>
        /// Returns the URI of a BHoM type, either by calculating it from the file structure of the main BHoM repository, or deriving it from the type's namespace. 
        /// This method must remain private and should never be used outside of the OntologyUri() method. Always use OntologyUri.
        /// </summary>
        private static Uri GithubURI(this Type typeToSearch, TBoxSettings tBoxSettings, LocalRepositorySettings repoSettings)
        {
            // Null guards
            if (typeToSearch == null || !typeToSearch.IsBHoMType())
                return null;

            repoSettings = repoSettings ?? new LocalRepositorySettings();

            // Custom types exception.
            if (typeToSearch is CustomObjectType)
            {
                Log.RecordError($"{typeToSearch.Name} is a {nameof(CustomObjectType)}, for which a GithubURI cannot be calculated. Make sure to use the {nameof(OntologyUri)} method instead.");
                return null;
            }

            // Try extracting the Github Uri by deriving it from a fileSystem search for a `.cs` file corresponding to the input Type.
            Uri result = GithubURIFromLocalRepository(typeToSearch, tBoxSettings, repoSettings);
            if (result != null)
                return result;

            // If the previous fails, compose a GitHub uri by leveraging namespace/folder conventions. This will fail wherever conventions are not respected.
            return GithubURIFromNamespace(typeToSearch);
        }

        /***************************************************/

        /// <summary>
        /// Returns the URI of a BHoM Type by calculating it from the file structure of the main BHoM repository. 
        /// This method must remain private and should never be used outside of the OntologyUri() method. Always use OntologyUri.
        /// </summary>
        private static Uri GithubURIFromLocalRepository(this Type typeToSearch, TBoxSettings tBoxSettings, LocalRepositorySettings settings)
        {
            if (typeToSearch.Name.StartsWith("<>c__"))
                return null;

            string typeFilePath = typeToSearch.FilePathFromLocalRepository(settings, true);

            if (!string.IsNullOrWhiteSpace(typeFilePath))
            {
                var pathComponents = typeFilePath.Split(Path.DirectorySeparatorChar).Where(pc => !string.IsNullOrWhiteSpace(pc)).ToList();

                try
                {
                    var assemblyDescriptionAttr = typeToSearch.Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>();
                    string githubToolkitUrl = assemblyDescriptionAttr.Description;
                    Uri githubToolkitUri = new Uri(githubToolkitUrl);

                    Uri URL = CombineUris(githubToolkitUri, "/blob/main/", string.Join("/", pathComponents.Skip(1)));
                    return URL;
                }
                catch (Exception e)
                {
                    Log.RecordWarning($"Could not compute the Uri from local repository for {typeToSearch}. Error: {e.ToString()}", true);
                }
            }

            return null;
        }

        /***************************************************/

        /// <summary>
        /// Returns the URI of a BHoM MemberInfo by calculating it from the file structure of the main BHoM repository. 
        /// This method must remain private and should never be used outside of the OntologyUri() method. Always use OntologyUri.
        /// </summary>
        private static Uri GithubURIFromLocalRepository(this MemberInfo pi, TBoxSettings tBoxSettings, LocalRepositorySettings settings)
        {
            if (pi == null)
                return null;

            Uri declaringTypeUri = pi.DeclaringType.OntologyUri(tBoxSettings, settings);

            int lineNumber = Compute.LineNumber(pi as dynamic, settings);

            if (lineNumber < 0)
                return declaringTypeUri;

            Uri result = new Uri($"{ declaringTypeUri.ToString()}#L{lineNumber + 1}"); // need to add +1 because Github Line numbers start from 1.

            return result;
        }

        /***************************************************/

        /// <summary>
        /// Returns the URI of a BHoM type by deriving it from the namespace of the type. 
        /// This method must remain private and should never be used outside of the GithubURI() method. Always use OntologyUri.
        /// </summary>
        private static Uri GithubURIFromNamespace(this Type t)
        {
            // Null guards
            if (t == null || !t.IsBHoMType())
                return null;

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
                Log.RecordError($"Could not compose a valid URL for type {t.FullName}", true);

            return uri;
        }
    }
}
