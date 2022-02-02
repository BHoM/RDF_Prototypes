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
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Modify
    {
        [Description("Modifies the Given RDF IGraph adding to it a URI Node, which is obtained by inferring the ")]
        public static OntologyClass CreateOntologyClass(this OntologyGraph graph, Type t)
        {
            if (!t.FullName.StartsWith("BH.oM"))
            {
                log.RecordError("This method only supports BHoM types.");
                return null;
            }

            return graph.CreateOntologyClass(UriFactory.Create(t.UriFromType()));
        }
    }
}
