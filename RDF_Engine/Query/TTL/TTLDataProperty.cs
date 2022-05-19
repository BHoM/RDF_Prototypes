using BH.oM.RDF;
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
        public static string TTLDataProperty(Type type, IEnumerable<string> parentClassNames = null, LocalRepositorySettings tBOXSettings = null)
        {
            return TTLClass(type.GithubURI(tBOXSettings ?? new LocalRepositorySettings()).ToString(), type.FullName, type.Name, parentClassNames);
        }

        public static string TTLDataProperty(string uri, string domain, string range, string label_en, IEnumerable<string> parentClassNames = null)
        {
            string composed = $"### {uri}";
            composed += $"\n:Name rdf:type owl:DatatypeProperty ;";
            composed += $"\n\t\trdfs:domain :{domain} ;";
            composed += $"\n\t\trdfs:range :{range} ;";
            composed += $"\n\t\trdfs:label :{label_en}@en .";

            return composed;
        }
    }
}
