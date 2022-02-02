
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Writes the given RDFDotNet IGraph in a file.")]
        public static void WriteToXMLFile(this IGraph graph, string directory, string filename)
        {
            WriteToXMLFile(graph, Path.Combine(directory, filename));
        }

        [Description("Writes the given RDFDotNet IGraph in a file.")]
        public static void WriteToXMLFile(this IGraph graph, string filePath = @"C:\temp\RDF_Prototypes_test\RDF_Prototypes_test.rdf")
        {
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(graph, filePath);
        }
    }
}
