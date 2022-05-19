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
        public static List<string> TTLClasses(this CSharpGraph cSharpGraph, LocalRepositorySettings localRepositorySettings)
        {
            List<string> TTLClasses = new List<string>();

            foreach (var classType in cSharpGraph.Classes)
            {
                string TTLClass = "";

                // Declaration with Uri
                string typeUri = classType.GithubURI(localRepositorySettings).ToString();
                TTLClass += $"### {typeUri}";

                // Class Identifier
                TTLClass += $"\n:{classType.UniqueNodeId()} rdf:type owl:Class;";

                // Subclasses
                List<Type> parentTypes = classType.ParentTypes().Where(t => t.IsOntologyClass()).ToList();

                foreach (Type subClass in parentTypes)
                {
                    TTLClass += $"\n\t\trdfs:subClassOf :{subClass.UniqueNodeId()};";
                }

                // Class label
                TTLClass += "\n\t\t" + $@"rdfs:label""{classType.DescriptiveName()}""@en .";

                TTLClasses.Add(TTLClass);
            }

            return TTLClasses;
        }
    }
}
