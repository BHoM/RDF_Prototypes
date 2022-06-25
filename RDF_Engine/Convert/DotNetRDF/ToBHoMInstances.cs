using BH.oM.Base;
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
    public static partial class Convert
    {
        public static List<object> ToBHoMInstances(this OntologyGraph dotNetRDFOntology)
        {
            var topIndividuals = dotNetRDFOntology.IndividualsNoOwner();

            List<object> result = new List<object>();

            foreach (OntologyResource individual in topIndividuals)
            {
                object bhomInstance = individual.ToBHoMInstance(dotNetRDFOntology);
                result.Add(bhomInstance);
            }

            return result;
        }

        public static List<object> ToBHoMInstances(this string TTLOntology)
        {
            return ToDotNetRDF(TTLOntology).ToBHoMInstances();
        }

        public static Uri AbsoluteUri(this INode node)
        {
            UriNode uriNode = node as UriNode;

            if (uriNode == null)
                return null;

            return new Uri(uriNode.Uri.AbsoluteUri);
        }

        public static string BHoMSegment(this INode node)
        {
            return node.AbsoluteUri()?.Segments?.LastOrDefault();
        }
    }
}