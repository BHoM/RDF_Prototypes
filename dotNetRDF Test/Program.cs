using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace dotNetRDF_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri serverURI = new Uri("http://localhost:7200");
            VDS.RDF.Storage.SparqlHttpProtocolConnector connector = new VDS.RDF.Storage.SparqlHttpProtocolConnector(serverURI);
        }
    }
}
