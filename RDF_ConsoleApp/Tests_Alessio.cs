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
using System.ComponentModel;
using VDS.RDF.Ontology;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static partial class Tests_Alessio
    {
        private static string _fileDirectory = @"C:\temp\RDF_Prototypes_test";

        [Description("Tries to create an ontology with a `Room` and `Column` class, to say that they both are subclass of `IObject`." +
            "TODO: This currently does not visualise in WebVOWL. Find out how to make it work.")]
        public static void RoomColumnSubClassOfIObject(TBoxSettings settings)
        {
            IGraph g = new Graph();

            // Set the base uri for the ontology.
            g.BaseUri = new Uri("https://github.com/BHoM/");

            // Set the namespace of our Ontology
            g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("http://visualdataweb.org/newOntology/"));
            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            // Create relationships nodes
            IUriNode owlClass = g.CreateUriNode("owl:Class");
            var a = g.CreateUriNode("rdf:type");
            var rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");

            // Create three Uri Nodes for an iObject, a Room and a Column
            IUriNode iObject = g.CreateUriNode(typeof(IObject), settings);
            IUriNode room = g.CreateUriNode(typeof(Room), settings);
            IUriNode column = g.CreateUriNode(typeof(Column), settings);

            // Set the is a class relationship
            g.Assert(iObject, a, owlClass);
            g.Assert(room, a, owlClass);
            g.Assert(column, a, owlClass);

            // Set the subclass relationship
            g.Assert(room, rdfsSubclassOf, iObject);
            g.Assert(column, rdfsSubclassOf, iObject);

            g.WriteToXMLFile(_fileDirectory, "RoomColumnSubclassOfIObject.rdf");
        }


        [Description("Tries to create an ontology with a `Room` and `Column` class, to say that they both are subclass of `IObject`." +
            "TODO: This currently does not visualise in WebVOWL. Find out how to make it work.")]
        public static void RoomColumnSubClassOfIObject_OntologyGraph(TBoxSettings settings)
        {
            OntologyGraph g = new OntologyGraph();

            // Set the base uri for the ontology.
            g.BaseUri = new Uri("https://github.com/BHoM/");

            // Set the namespace of our Ontology
            g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("https://github.com/BHoM/"));
            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            // Create relationships nodes
            IUriNode a = g.CreateUriNode("rdf:type");
            IUriNode rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");

            // Create three Uri Nodes for an iObject, a Room and a Column
            OntologyClass iObject = g.CreateOntologyClass(typeof(IObject), settings);
            OntologyClass room = g.CreateOntologyClass(typeof(Room), settings);
            OntologyClass column = g.CreateOntologyClass(typeof(Column), settings);

            //// Set the is a class relationship
            //g.Assert(iObject, a, iObject_owlClass);
            //g.Assert(room, a, room_owlClass);
            //g.Assert(column, a, column_owlClass);

            //// Set the subclass relationship
            //g.Assert(room, rdfsSubclassOf, iObject);
            //g.Assert(column, rdfsSubclassOf, iObject);

            g.WriteToXMLFile(_fileDirectory, "RoomColumnSubclassOfIObject.rdf");
        }
    }
}
