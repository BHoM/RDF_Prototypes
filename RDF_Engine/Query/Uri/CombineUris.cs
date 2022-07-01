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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Combine the given strings and base uri in a single absolute Uri. More reliable than standard Uri.Combine.")]
        public static Uri CombineUris(this Uri baseUri, params string[] uris)
        {
            if (baseUri == null)
                return null;

            string combinedUriString = baseUri.ToString();

            if (combinedUriString.EndsWith("/"))
                combinedUriString = combinedUriString.Remove(combinedUriString.Length - 1, 1);

            foreach (string uri in uris)
            {
                if (uri == null)
                    continue;

                string correctedUri = uri.Replace(@"\", "/");
                if (correctedUri.StartsWith("/"))
                    correctedUri = correctedUri.Remove(0, 1);

                if (correctedUri.EndsWith("/"))
                    correctedUri = correctedUri.Remove(correctedUri.Length - 1, 1);

                combinedUriString += "/" + correctedUri;
            }

            Uri result = null;
            Uri.TryCreate(combinedUriString, UriKind.Absolute, out result);

            return result;
        }

        /***************************************************/

        public static Uri CombineUris(params string[] uris)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder = new UriBuilder(uris.FirstOrDefault());

            return CombineUris(uriBuilder.Uri, uris.Skip(1).ToArray());
        }
    }
}