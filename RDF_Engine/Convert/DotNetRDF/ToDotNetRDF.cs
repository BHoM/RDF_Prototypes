
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.RDF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Ontology;
using VDS.RDF.Parsing;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Converts a TTL string to a DotNetRDF graph object called OntologyGraph.")]
        public static OntologyGraph ToDotNetRDF(string ttlGraph)
        {
            OntologyGraph g = new OntologyGraph();

            TurtleParser turtleParser = new TurtleParser();
            TextReader reader = new StringReader(ttlGraph);

            turtleParser.Load(g, reader);

            if (new OntologyGraph().Equals(g))
                return null;

            return g;
        }
    }
}
