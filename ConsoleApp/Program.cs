using BH.oM.Analytical.Results;
using BH.oM.Base;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.External.RDF;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.oM.External.RDF;
using BH.oM.Architecture.Elements;
using BH.oM.Physical.Elements;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    class Program
    {
        public static void Main(string[] args = null)
        {
            //List<Assembly> oMassemblies = BH.Engine.External.RDF.Compute.LoadAssembliesInDirectory(true);

            // HashSet<TypeInfo> oMTypes = new HashSet<TypeInfo>(oMassemblies.SelectMany(a => a.DefinedTypes));

            HasElement<Room, Column> relation = new HasElement<Room, Column>();
            Room room = new Room();
            Column column = new Column();

            relation.Subject = room;
            relation.Object = column;

            IGraph g = new Graph();
            //var subjectNode = g.CreateLiteralNode(room.GetType().FullName);
            var subjectNode = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Architecture_oM/Elements/Room.cs"));
            var predicateNode = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Architecture_oM/Elements/Room.cs")); // change to address for relationship code
            var objectNode = g.CreateLiteralNode(column.GetType().FullName);

            g.Assert(new Triple(subjectNode, predicateNode, objectNode));

            WriteGraphToXml(g);
        }

        public static void DotNetRDF_test()
        {
            IGraph g = new Graph();

            IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
            IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
            ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

            g.Assert(new Triple(dotNetRDF, says, helloWorld));
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));

            //foreach (Triple t in g.Triples)
            //{
            //    Console.WriteLine(t.ToString());
            //}
            //Console.ReadLine();

            WriteGraphToJson(g);

            Console.ReadLine();
        }


        public static void WriteGraphToJson(IGraph graph, string directory = @"C:\temp\RDF_Prototypes_test", string filename = "RDF_Prototypes_test.rdf")
        {
            RdfJsonWriter rdfJsonWriter = new RdfJsonWriter();
            System.IO.StringWriter sw = new System.IO.StringWriter();

            //Call the Save() method to write to the StringWriter
            rdfJsonWriter.Save(graph, sw);

            //We can now retrieve the written RDF by using the ToString() method of the StringWriter
            String data = sw.ToString();
            Console.WriteLine(data);

            System.IO.Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, $"RDF_Prototypes_test.json"), data);
        }

        public static void WriteGraphToXml(IGraph graph, string directory = @"C:\temp\RDF_Prototypes_test", string filename = "RDF_Prototypes_test.rdf")
        {
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter(); 
            rdfxmlwriter.Save(graph, Path.Combine(directory, filename));
        }
    }
}
