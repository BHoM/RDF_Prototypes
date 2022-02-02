using BH.Engine.RDF;
using BH.oM.Architecture.Elements;
using BH.oM.Base;
using BH.oM.Physical.Elements;
using BH.oM.RDF;
using System.ComponentModel;
using VDS.RDF;
using VDS.RDF.Writing;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static partial class Tests
    {
        private static string _fileDirectory = @"C:\temp\RDF_Prototypes_test";

        [Description("Writes a `Hello World` test JSON ontology file, as per the dotnetrdf.org tutorial example.")]
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

        [Description("Tests a new type of BHoM class called `Triple`. This is a prototype for a possible way of defining relations, kind of abandoned now.")]
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

        [Description("Code from Diellza: Tries to create owl:Classes (Structure_oM_Elements_Bar and Physical_oM_Elements_Beam) as subject and objects connectected by owl:ObjectProperties as predicate (hasElement or isPartOf). Uses owl and rdf vocabulary.")]
        public static void RoomWithBar()
        {
            IGraph g = new Graph();

            // Set the namespace of our Ontology
            // g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("http://visualdataweb.org/newOntology/"));

            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            var a = g.CreateUriNode("rdf:type");
            var owlClass = g.CreateUriNode("owl:Class");
            var owlObjectProperty = g.CreateUriNode("owl:ObjectProperty");
            var rdfsLabel = g.CreateUriNode("rdfs:label");
            var rdfsDomain = g.CreateUriNode("rdfs:domain");
            var rdfsRange = g.CreateUriNode("rdfs:range");
            var rdfsSubPropertyOf = g.CreateUriNode("rdfs:subPropertyOf");


            var perimeter = g.CreateUriNode("rdfs:literal");
            var rdfsLiteral = g.CreateUriNode("rdfs:literal");


            //var rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");


            IUriNode Structure_oM_Elements_Bar = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Structure_oM/Elements/Bar.cs"));
            g.Assert(Structure_oM_Elements_Bar, a, owlClass);
            g.Assert(Structure_oM_Elements_Bar, rdfsLabel, g.CreateLiteralNode("Structure_oM_Elements_Bar", "en"));

            g.Assert(Structure_oM_Elements_Bar, rdfsLiteral, perimeter);


            IUriNode Physical_oM_Elements_Beam = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Physical_oM/Elements/Beam.cs"));
            g.Assert(Physical_oM_Elements_Beam, a, owlClass);
            g.Assert(Physical_oM_Elements_Beam, rdfsLabel, g.CreateLiteralNode("Physical_oM_Elements_Beam", "en"));



            // Relation
            IUriNode isPartOf = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/Predicate/isPartOf"));
            g.Assert(isPartOf, a, owlObjectProperty);
            g.Assert(isPartOf, rdfsLabel, g.CreateLiteralNode("isPartOf", "en"));

           // g.Assert(isPartOf, rdfsDomain, Physical_oM_Elements_Beam);
           // g.Assert(isPartOf, rdfsRange, Structure_oM_Elements_Bar);



            // Relation
            IUriNode hasElement = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/Predicate/hasElement"));
            g.Assert(hasElement, a, owlObjectProperty);
            g.Assert(hasElement, rdfsLabel, g.CreateLiteralNode("hasElement", "en"));
            g.Assert(hasElement, rdfsSubPropertyOf, isPartOf);


            g.Assert(hasElement, rdfsDomain, Structure_oM_Elements_Bar);
            g.Assert(hasElement, rdfsRange, Structure_oM_Elements_Bar);
     
                            

           g.WriteToXMLFile(_fileDirectory, "RoomWithBar.rdf");



        }

        [Description("Tries to create an ontology with a `Room` and `Column` class, to say that they both are subclass of `IObject`." +
            "TODO: This currently does not visualise in WebVOWL. Find out how to make it work.")]
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

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, "RoomColumnSubclassOfI.rdf");
        }
    }
}
