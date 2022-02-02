using System;
using System.Collections.Generic;
using BH.Engine.RDF;
using BH.oM.Architecture.Elements;
using BH.oM.Base;
using BH.oM.Physical.Elements;
using BH.oM.RDF;
using System.ComponentModel;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Ontology;
using VDS.RDF.Writing;


namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static partial class Tests_Diellza
    {
        private static string _fileDirectory = @"C:\temp\RDF_Prototypes_test";

       
        [Description("Code from Diellza: Tries to create owl:Classes (Structure_oM_Elements_Bar and Physical_oM_Elements_Beam) as subject and objects connectected by owl:ObjectProperties as predicate (hasElement or isPartOf). Uses owl and rdf vocabulary.")]
        public static void RoomWithBar()
        {
            IGraph g = new Graph();

            // Set the base uri for the ontology.
            //g.BaseUri = new Uri("https://github.com/BHoM/");

            // Set the namespace of our Ontology
            // g.NamespaceMap.AddNamespace("bhom", UriFactory.Create("http://visualdataweb.org/newOntology/"));

            g.NamespaceMap.AddNamespace("owl", UriFactory.Create("http://www.w3.org/2002/07/owl"));

            var a = g.CreateUriNode("rdf:type");
            var owlClass = g.CreateUriNode("owl:class");
            var owlObjectProperty = g.CreateUriNode("owl:ObjectProperty");
            var rdfsLabel = g.CreateUriNode("rdfs:label");
            var rdfsDomain = g.CreateUriNode("rdfs:domain");
            var rdfsRange = g.CreateUriNode("rdfs:range");

            //var rdfsSubPropertyOf = g.CreateUriNode("rdfs:subPropertyOf");
            // var perimeter = g.CreateUriNode("rdfs:literal");
            // var rdfsLiteral = g.CreateUriNode("rdfs:literal");
            //var rdfsSubclassOf = g.CreateUriNode("rdfs:subclassOf");
                        
              
                    
           var Physical_oM_Elements_Beam = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Physical_oM/Elements/Beam.cs"));
            g.Assert(Physical_oM_Elements_Beam, a, owlClass);
            g.Assert(Physical_oM_Elements_Beam, rdfsLabel, g.CreateLiteralNode("Physical_oM_Elements_Beam", "en"));

            var Structure_oM_Elements_Bar = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/BHoM/blob/main/Structure_oM/Elements/Bar.cs"));
            g.Assert(Structure_oM_Elements_Bar, a, owlClass);
            g.Assert(Structure_oM_Elements_Bar, rdfsLabel, g.CreateLiteralNode("Structure_oM_Elements_Bar", "en"));


            // Relation
            var isPartOf = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/Predicate/isPartOf"));
            g.Assert(isPartOf, a, owlObjectProperty);
            g.Assert(isPartOf, rdfsLabel, g.CreateLiteralNode("isPartOf", "en"));

            g.Assert(isPartOf, rdfsDomain, Structure_oM_Elements_Bar);
            g.Assert(isPartOf, rdfsRange, Physical_oM_Elements_Beam);

            //Relation
            //var hasElement = g.CreateUriNode(UriFactory.Create("https://github.com/BHoM/Predicate/hasElement"));
            //g.Assert(hasElement, a, owlObjectProperty);
            //g.Assert(hasElement, rdfsLabel, g.CreateLiteralNode("hasElement", "en"));
            

            // g.Assert(hasElement, rdfsDomain, Structure_oM_Elements_Bar);
            // g.Assert(hasElement, rdfsRange, Structure_oM_Elements_Bar);

            g.WriteToXMLFile(_fileDirectory, "RoomWithBar.rdf");
        }
    }
}
