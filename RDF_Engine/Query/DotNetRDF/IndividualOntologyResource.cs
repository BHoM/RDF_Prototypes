using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Ontology;

namespace BH.Engine.RDF
{
    public static partial class Query
    {
        [Description("")]
        public static OntologyResource IndividualOntologyResource(this INode node, OntologyGraph dotNetRDFOntology)
        {
            if (node == null || dotNetRDFOntology == null || !(node is UriNode))
                return null;

            OntologyResource individualResource = dotNetRDFOntology.Individuals().Where(or => (or.Resource as UriNode)?.Uri == (node as UriNode)?.Uri).FirstOrDefault();

            return individualResource;
        }

    }
}