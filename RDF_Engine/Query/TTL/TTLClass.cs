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
        public static string TTLClass(this Type type, LocalRepositorySettings localRepoSettings = null)
        {
            List<Type> parentTypes = type.ParentTypes();

            IEnumerable<string> parentClassUniqueIds = parentTypes.Select(t => t.UniqueNodeId());

            return TTLClass(type.GithubURI(localRepoSettings ?? new LocalRepositorySettings()).ToString(), type.FullName, type.Name, parentClassUniqueIds);
        }

        public static string TTLClass(string uri, string uniqueClassName, string en_label, IEnumerable<string> parentClassUniqueIds = null)
        {
            string composed = $"### {uri}";
            composed += $"\n:{uniqueClassName} rdf:type owl:Class ;" + ";";
            composed += ";";
            foreach (var parentClassUniqueId in parentClassUniqueIds)
            {
                composed += "\n\t\t" + $"rdfs: subClassOf: :{parentClassUniqueId}" + ";";
            }

            composed += "\n\t\t" + $@"rdfs: label ""{en_label}""@en .";

            return composed;
        }
    }
}
