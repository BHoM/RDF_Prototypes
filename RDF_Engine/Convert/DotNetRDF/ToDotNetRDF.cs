
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

            try
            {
                turtleParser.Load(g, reader);
            }
            catch (Exception e)
            {
                Log.RecordError($"Could not convert textual TTL graph to a DotNetRDF OntologyGraph. Error:\n\t{e.ToString().SplitInLinesAndTabify()}");
            }

            if (new OntologyGraph().Equals(g))
                return null;

            return g;
        }
    }
}
