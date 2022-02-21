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
            IUriNode iObject = g.CreateUriNode(typeof(IObject));
            IUriNode room = g.CreateUriNode(typeof(Room));
            IUriNode column = g.CreateUriNode(typeof(Column));

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
            OntologyClass iObject = g.CreateOntologyClass(typeof(IObject));
            OntologyClass room = g.CreateOntologyClass(typeof(Room));
            OntologyClass column = g.CreateOntologyClass(typeof(Column));

            //// Set the is a class relationship
            //g.Assert(iObject, a, iObject_owlClass);
            //g.Assert(room, a, room_owlClass);
            //g.Assert(column, a, column_owlClass);

            //// Set the subclass relationship
            //g.Assert(room, rdfsSubclassOf, iObject);
            //g.Assert(column, rdfsSubclassOf, iObject);

            g.WriteToXMLFile(_fileDirectory, "RoomColumnSubclassOfIObject.rdf");
        }

        public static void WriteWebVOWLOntologiesPerNamespace(string saveDirRelativeToRepoRoot = "WebVOWLOntology")
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            List<TypeInfo> oMTypes = oMassemblies.BHoMTypes();

            // Take a subset of the types avaialble to reduce the size of the output graph. This can become a Filter function.
            //IEnumerable<TypeInfo> onlyBaseOmTypes = oMTypes.Where(t => t != null && t.Namespace != null && t.Namespace.EndsWith("BH.oM.Base")).ToList();
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name == "NamedNumericTolerance" || t.Name == "IObject");
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("Output"));
            //onlyBaseOmTypes = onlyBaseOmTypes.Where(t => t.Name.Contains("ComparisonConfig"));

            SortedDictionary<string, string> webVOWLJsonsPerNamespace = Engine.RDF.Compute.WebVOWLJsonPerNamespace(oMTypes);

            // Save all generated ontologies to file
            foreach (var kv in webVOWLJsonsPerNamespace)
                kv.Value.WriteToJsonFile($"{kv.Key}.json", $"..\\..\\..\\{saveDirRelativeToRepoRoot}");

            // Save the URLS to the ontologies. These are links to the WebVOWL website with a parameter passed that links directly the Github URL of the ontology.
            string allWebOWLOntologyURL = $"..\\..\\..\\{saveDirRelativeToRepoRoot}\\_allWebOWLOntologyURL.txt";

            File.WriteAllText(allWebOWLOntologyURL, ""); // empty the file
            foreach (var kv in webVOWLJsonsPerNamespace)
            {
                string WebVOWLOntologyURL = $"https://service.tib.eu/webvowl/#url=https://raw.githubusercontent.com/BHoM/RDF_Prototypes/main/{saveDirRelativeToRepoRoot}/{kv.Key}.json";
                File.AppendAllText(allWebOWLOntologyURL, "\n" + WebVOWLOntologyURL);
            }
        }

        public static void WriteWebVOWLOntology(List<string> typeFullNames, string fileName = null, string saveDirRelativeToRepoRoot = "WebVOWLOntology", HashSet<string> exceptions = null)
        {
            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            
            // Get the System.Types corresponding to the input typeFullNames
            List<Type> correspondingOmTypes = oMassemblies.BHoMTypes().Where(t => typeFullNames.Contains(t.AsType().FullName)).Select(ti => ti.AsType()).ToList();

            WriteWebVOWLOntology(correspondingOmTypes, fileName, saveDirRelativeToRepoRoot, exceptions);
        }

        public static void WriteWebVOWLOntology(List<Type> types, string fileName = null, string saveDirRelativeToRepoRoot = "WebVOWLOntology", HashSet<string> exceptions = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Join("-", types.Select(t => t.Name));

            // Of all the input System.Types, get also all the BHoM System.Types of all their Properties.
            //HashSet<Type> allConnectedBHoMTypes = types.AllNestedTypes();
            //types = types.Concat(allConnectedBHoMTypes).Distinct().ToList();

            List<Assembly> oMassemblies = BH.Engine.RDF.Compute.LoadAssembliesInDirectory(true);
            List<TypeInfo> oMTypeInfos = oMassemblies.BHoMTypes().Where(t => types.Contains(t.AsType())).ToList();

            WriteWebVOWLOntology(oMTypeInfos, fileName, saveDirRelativeToRepoRoot, exceptions);
        }


        private static void WriteWebVOWLOntology(List<TypeInfo> oMTypes, string fileName, string saveDirRelativeToRepoRoot, HashSet<string> exceptions = null)
        {
            Dictionary<TypeInfo, List<IRelation>> dictionaryGraph = oMTypes.DictionaryGraphFromTypeInfos();
            string webVOWLJson = Engine.RDF.Convert.ToWebVOWLJson(dictionaryGraph, new HashSet<string>(oMTypes.Select(t => t.Namespace)), exceptions);

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = string.Join("-", oMTypes.Select(t => t.Name));

            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            webVOWLJson.WriteToJsonFile(fileName, $"..\\..\\..\\{saveDirRelativeToRepoRoot}");
        }
    }
}
