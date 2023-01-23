/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

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
using DescriptionAttribute = NUnit.Framework.DescriptionAttribute;

namespace BH.Test.RDF
{
    public static partial class WebVOWL_Tests
    {
        //[Test]
        public static void WebOwlOntology()
        {
            LocalRepositorySettings repoSettings = new LocalRepositorySettings();
            repoSettings.WriteCacheFiles = true;

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Base.BHoMObject",
            }, repoSettings, relationRecursion: 0);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Base.BHoMObject",
            }, repoSettings, relationRecursion: 1);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Base.BHoMObject",
            }, repoSettings, relationRecursion: 2);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Physical.Elements.Column",
            }, repoSettings, relationRecursion: 0);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
            }, repoSettings, relationRecursion: 0);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
            }, repoSettings, relationRecursion: 1);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
            }, repoSettings, relationRecursion: 2);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
                "BH.oM.Physical.Elements.Wall",
                "BH.oM.Architecture.Elements.Room",
                "BH.oM.Physical.Elements.Column"
            }, repoSettings, relationRecursion: 1);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
                "BH.oM.Physical.Elements.Wall",
                "BH.oM.Architecture.Elements.Room",
                "BH.oM.Physical.Elements.Column"
            }, repoSettings, relationRecursion: 2);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
                "BH.oM.Physical.Elements.Wall",
                "BH.oM.Architecture.Elements.Room",
                "BH.oM.Physical.Elements.Column"
            }, repoSettings, relationRecursion: 99);


            Engine.RDF.Compute.WriteWebVOWLOntologiesPerNamespace(repoSettings);
        }

        //[Test]
        [Description("Tries to create an ontology with a `Room` and `Column` class, to say that they both are subclass of `IObject`." +
            "TODO: This currently does not visualise in WebVOWL. Find out how to make it work.")]
        public static void RoomColumnSubClassOfIObject()
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

        //[Test]
        [Description("Tries to create an ontology with a `Room` and `Column` class, to say that they both are subclass of `IObject`." +
            "TODO: This currently does not visualise in WebVOWL. Find out how to make it work.")]
        public static void RoomColumnSubClassOfIObject_OntologyGraph()
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

        private static LocalRepositorySettings settings = new LocalRepositorySettings();
        private static string _fileDirectory = @"C:\temp\RDF_Prototypes_test";
    }
}
