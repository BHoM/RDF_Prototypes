using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("Attempts to parse the input string with a TTL parser and get the individuals.")]
        public static List<OntologyResource> Individuals(this string TTLGraph)
        {
            OntologyGraph g = Convert.ToDotNetRDF(TTLGraph);

            if (g != null)
                return g.Individuals();
            else
                Log.RecordError("The input string was not a correctly formed TTL graph.");

            return null;
        }

        public static List<OntologyResource> IndividualsNoOwner(this OntologyGraph ontologyGraph)
        {
            return ontologyGraph.Individuals().Where(i => !i.TriplesWithObject.Any()).ToList();
        }

        [Description("Gets all Individuals nodes from a DotNetRDF OntologyGraph object.")]
        public static List<OntologyResource> Individuals(this OntologyGraph ontologyGraph)
        {
            if (ontologyGraph == null)
                return new List<OntologyResource>();

            List<OntologyResource> individuals = ontologyGraph.AllClasses.Select(c => c.Instances.Where(i => i != null).ToList()).Where(s => s.Any()).SelectMany(l => l).ToList(); //g.Triples.Where(t => )

            return individuals;
        }
    }
}