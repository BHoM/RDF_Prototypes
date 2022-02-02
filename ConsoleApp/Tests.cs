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
using BH.Engine.RDF;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.oM.Architecture.Elements;
using BH.oM.Physical.Elements;
using BH.oM.RDF;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static partial class Tests
    {
        private static string _fileDirectory = @"C:\temp\RDF_Prototypes_test";

        public static void HelloWorld_Test_Json()
        {
            IGraph g = new Graph();

            IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
            IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
            ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

            g.Assert(new Triple(dotNetRDF, says, helloWorld));
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));

            g.WriteToJsonFile(_fileDirectory, "HelloWorld_Test_Json.json");
        }

        public static void Triple_Test_XML()
        {
            HasElement hasElementRelation = new HasElement();
            Room room = new Room();
            Column column = new Column();

            string roomUri = room.GetType().UriFromType();
            string columnUri = column.GetType().UriFromType();

            hasElementRelation.Subject = room;
            hasElementRelation.Object = column;

            IGraph g = new Graph();
            //var subjectNode = g.CreateLiteralNode(room.GetType().FullName);
            var subjectNode = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Architecture_oM/Elements/Room.cs"));
            var predicateNode = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Architecture_oM/Elements/Room.cs")); // change to address for relationship code
            var objectNode = g.CreateLiteralNode(column.GetType().FullName);

            g.Assert(new Triple(subjectNode, predicateNode, objectNode));

            g.WriteToXMLFile(_fileDirectory, "Triple_Test_XML.rdf");
        }

        // Code from Diellza
        public static void RoomWithBar()
        {
            IGraph g = new Graph();

            // Set the namespace of our Ontology
            g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("http://visualdataweb.org/newOntology/"));
            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            var a = g.CreateUriNode("rdf:type");
            var owlClass = g.CreateUriNode("owl:Class");
            var owlObjectProperty = g.CreateUriNode("owl:ObjectProperty");
            var rdfsLabel = g.CreateUriNode("rdfs:label");
            var rdfsDomain = g.CreateUriNode("rdfs:domain");
            var rdfsRange = g.CreateUriNode("rdfs:range");
            var rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");
            var rdfsCharac = g.CreateUriNode("rdfs:charac");

            IUriNode BHoM_Architecture_oM_Elements_Room = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Architecture_oM/Elements/Room.cs"));
            g.Assert(BHoM_Architecture_oM_Elements_Room, a, owlClass);
            g.Assert(BHoM_Architecture_oM_Elements_Room, rdfsLabel, g.CreateLiteralNode("BHoM_Architecture_oM_Elements_Room", "en"));

            IUriNode Structure_oM_Elements_Bar = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Structure_oM/Elements/Bar.cs"));
            g.Assert(Structure_oM_Elements_Bar, a, owlClass);
            g.Assert(Structure_oM_Elements_Bar, rdfsLabel, g.CreateLiteralNode("Structure_oM_Elements_Bar", "en"));

            // Relation
            IUriNode isPartOf = g.CreateUriNode("rdf:type");
            g.Assert(isPartOf, a, owlObjectProperty);
            g.Assert(isPartOf, rdfsLabel, g.CreateLiteralNode("isPartOf", "en"));
            g.Assert(isPartOf, rdfsDomain, BHoM_Architecture_oM_Elements_Room);
            g.Assert(isPartOf, rdfsRange, Structure_oM_Elements_Bar);

            g.WriteToXMLFile(_fileDirectory, "RoomWithBar.rdf");
        }

        public static void RoomColumnSubClassOfIObject()
        {
            IGraph g = new Graph();

            // Set the namespace of our Ontology
            g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("http://visualdataweb.org/newOntology/"));
            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            var rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");

            IUriNode iObject = g.CreateUriNode(typeof(IObject));
            IUriNode room = g.CreateUriNode(typeof(Room));
            IUriNode column = g.CreateUriNode(typeof(Column));

            g.Assert(room, rdfsSubclassOf, iObject);
            g.Assert(column, rdfsSubclassOf, iObject);

            g.WriteToXMLFile(_fileDirectory, "RoomColumnSubclassOfIObject.rdf");
        }

        public static void RoomColumn()
        {
            IGraph g = new Graph();

            // Set the namespace of our Ontology
            g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("http://visualdataweb.org/newOntology/"));
            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            var a = g.CreateUriNode("rdf:type");
            var owlObjectProperty = g.CreateUriNode("owl:ObjectProperty");
            var rdfsLabel = g.CreateUriNode("rdfs:label");
            var rdfsDomain = g.CreateUriNode("rdfs:domain");
            var rdfsRange = g.CreateUriNode("rdfs:range");
            var rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");
            var rdfsCharac = g.CreateUriNode("rdfs:charac");

            IUriNode room = g.CreateUriNode(typeof(Room));
            IUriNode column = g.CreateUriNode(typeof(Column));

            IUriNode Structure_oM_Elements_Bar = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Structure_oM/Elements/Bar.cs"));
            g.Assert(Structure_oM_Elements_Bar, rdfsLabel, g.CreateLiteralNode("Structure_oM_Elements_Bar", "en"));

            // Relation
            IUriNode isPartOf = g.CreateUriNode("rdf:type");
            g.Assert(isPartOf, a, owlObjectProperty);
            g.Assert(isPartOf, rdfsLabel, g.CreateLiteralNode("isPartOf", "en"));
            g.Assert(isPartOf, rdfsDomain, room);
            g.Assert(isPartOf, rdfsRange, Structure_oM_Elements_Bar);

            g.WriteToXMLFile(@"C:\temp\RDF_Prototypes_test\RoomColumn.rdf");
        }
    }
}
