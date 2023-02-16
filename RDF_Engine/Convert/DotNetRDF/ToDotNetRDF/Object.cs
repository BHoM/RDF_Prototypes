
using BH.oM.Base;
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
using BH.Engine.Base;
using BH.oM.RDF;
using BH.oM.Base.Attributes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Xml.Linq;
using VDS.RDF.Parsing;
using VDS.RDF.Writing.Formatting;

namespace BH.Engine.Adapters.RDF
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        // Kept private whilst prototyping - either to be removed or made public.

        [Description("Computes a DotNetRDF Graph with the input IObjects via DotNetRDF. Experimental.")]
        private static Graph Experimental_ToDotNetRDF(this object obj, string defaultIri = "http://example.rdf/")
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return json.XmlOrJsonTextToJsonData().ToRDFGraph(defaultIri);
        }

        [Description("Computes a TTL ontology with the input IObjects via DotNetRDF. Experimental.")]
        private static void Experimental_WriteTTLFile(this object obj, string filepath = @"C:/temp/test.ttl", string defaultIri = "http://example.rdf/")
        {
            Graph g = obj.Experimental_ToDotNetRDF(defaultIri);
            CompressingTurtleWriter t = new CompressingTurtleWriter();
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            t.Save(g, "C:/temp/test.ttl");
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<string, JToken> XmlOrJsonFilenameToJsonData(this string inputFilename)
        {
            var inputText = File.ReadAllText(inputFilename);
            return XmlOrJsonTextToJsonData(inputText);
        }

        private static Dictionary<string, JToken> XmlOrJsonTextToJsonData(this string inputText)
        {
            var jsonString = XmlOrJsonToJsonString(inputText);
            return DeserializeJsonString(jsonString);
        }

        private static Dictionary<string, JToken> DeserializeJsonString(this string jsonText)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, JToken>>(jsonText);
        }

        private static string XmlOrJsonToJsonString(this string inputText)
        {
            var output = inputText;
            try
            {
                var doc = XDocument.Parse(inputText);
                output = JsonConvert.SerializeXNode(doc);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }

            return output;
        }
    }

    public static class ModelToRdf
    {
        public static string DefaultIri { get; set; } = @"http://model2.rdf/";

        public static void SerializeNTriples(this Graph graph, string outputFilename)
        {
            var triples = graph.GraphToNTriples();
            //var writer = new NTriplesWriter(NTriplesSyntax.Rdf11) { SortTriples = true};
            Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));
            File.WriteAllLines(outputFilename, triples);
            //writer.Save(graph, outputFilename);
        }

        public static IEnumerable<string> GraphToNTriples(this Graph graph)
        {
            //Create a formatter
            ITripleFormatter formatter = new NTriplesFormatter(NTriplesSyntax.Rdf11);

            //Print triples with this formatter
            foreach (var t in graph.Triples) yield return t.ToString(formatter);
        }

        //TODO: Should convert to a custom object (json/dictionary?), to avoid cyclic calls.
        public static Graph ToRDFGraph(this IDictionary<string, JToken> jDictionary, string defaultIri = "")
        {
            if (!string.IsNullOrWhiteSpace(defaultIri)) DefaultIri = defaultIri;
            var graph = new Graph();
            jDictionary.ToRDFGraph(graph);
            return graph;
        }

        //TODO: This should be external. The user should be able to specify what is the Id.
        internal static string GetId(this IDictionary<string, JToken> jDictionary)
        {
            var idPairs = jDictionary.Where(x => x.Key.ToLower() == "id" || x.Key.ToLower() == "@id");
            if (idPairs.Any()) return idPairs.FirstOrDefault().Value.ToString();
            return Guid.NewGuid().ToString();
        }

        internal static void ToRDFGraph(this IDictionary<string, JToken> jDictionary, Graph graph)
        {
            if (jDictionary is null) return;

            var id = jDictionary.GetId();

            var entityNode = id.ToUriNode(DefaultIri);

            jDictionary.ToRDFGraph(graph, entityNode);
        }

        internal static void ToRDFGraph(this JToken jToken, string key, IUriNode entityNode, Graph graph)
        {
            //TODO: This is very specific to E3 (==0)
            //if (string.IsNullOrWhiteSpace(jToken.ToString()) || jToken.ToString().Equals("0"))
            //    return;
            //TODO: Implement: If has attribute "Id";
            if (key.ToLower().Equals("id") || key.ToLower().Equals("@id"))
            {
                graph.Assert(entityNode, key.ToUriNode(DefaultIri), jToken.ToString().ToLiteralNode());
            }
            else if (key.ToLower().EndsWith("id") || key.ToLower().EndsWith("ids"))
            {
                graph.Assert(entityNode, key.ToUriNode(DefaultIri), jToken.ToString().ToUriNode(DefaultIri));
            }
            else
            {
                //TODO: Create a node for each type:
                switch (jToken.Type)
                {
                    case JTokenType.Date:
                        var dateTime = DateTime.Parse(jToken.ToString());
                        graph.Assert(entityNode, key.ToUriNode(DefaultIri), dateTime.ToString("yyyy-MM-dd HH:mm:ss").ToLiteralNode());
                        break;
                    case JTokenType.None:
                    case JTokenType.Object:
                    case JTokenType.Array:
                    case JTokenType.Constructor:
                    case JTokenType.Property:
                    case JTokenType.Comment:
                    case JTokenType.Integer:
                    case JTokenType.Float:
                    case JTokenType.String:
                    case JTokenType.Boolean:
                    case JTokenType.Null:
                    case JTokenType.Undefined:
                    case JTokenType.Raw:
                    case JTokenType.Bytes:
                    case JTokenType.Guid:
                    case JTokenType.Uri:
                    case JTokenType.TimeSpan:
                    default:
                        graph.Assert(entityNode, key.ToUriNode(DefaultIri), jToken.ToString().ToLiteralNode());
                        break;
                }

            }
        }

        //TODO: This should be external. The user should be able to specify which properties to map and how.
        //TODO: Add ClassName as a Predicate, with the 'model.GetType().Name'
        internal static void ToRDFGraph(this IDictionary<string, JToken> model, Graph graph, IUriNode entityNode)
        {
            if (model is null) return;

            var keys = model.Keys;

            foreach (var key in keys)
            {
                var value = model[key];
                if (value == null) return;

                if (value.Type.ToString().Equals("Array"))
                {
                    if (value is JArray jArray)
                        foreach (var item in jArray)
                            if (item is IDictionary<string, JToken> itemDict)
                            {
                                //TODO: Add reference to this object:
                                var id = itemDict.GetId().ToUriNode(DefaultIri);
                                graph.Assert(entityNode, key.ToUriNode(DefaultIri), id);
                                itemDict.ToRDFGraph(graph, id);
                            }
                            else
                            {
                                if (item is JToken itemToken)
                                    itemToken.ToRDFGraph(key, entityNode, graph);
                                else
                                    throw new Exception($"Unhandled scenario: Key: {key}; Data: {item}");
                            }
                    else
                        throw new Exception($"Unhandled scenario: Key: {key}; Data: {value}");
                }
                else if (value.Type.ToString().Equals("Object"))
                {
                    if (value is IDictionary<string, JToken> itemDict)
                    {
                        var id = Guid.NewGuid().ToString().ToUriNode(DefaultIri);
                        graph.Assert(entityNode, key.ToUriNode(DefaultIri), id);
                        itemDict.ToRDFGraph(graph, id);
                    }
                    else
                    {
                        throw new Exception($"Unhandled scenario: Key: {key}; Data: {value}");
                    }
                }
                else
                {
                    value.ToRDFGraph(key, entityNode, graph);
                }
            }
        }
    }

    public static class RdfExtensions
    {
        public static bool IsLiteral(this INode node) => node.NodeType.Equals(NodeType.Literal);
        public static ILiteralNode ToLiteralNode(this string value) => new NodeFactory().CreateLiteralNode(value);
        public static ILiteralNode ToLiteralNode(this bool value) => new NodeFactory().CreateLiteralNode(value.ToString());

        public static Uri ToUri(this string value, string iri) => UriFactory.Create($"{iri}{value}");
        public static Uri ToUri(this int value, string iri) => UriFactory.Create($"{iri}{value}");
        public static Uri ToUri(this string value) => UriFactory.Create(value);
        public static Uri ToUri(this int value) => UriFactory.Create(value.ToString());
        public static IUriNode ToUriNode(this string value) => new NodeFactory().CreateUriNode(value.ToUri());
        public static IUriNode ToUriNode(this int value) => new NodeFactory().CreateUriNode(value.ToUri());
        public static IUriNode ToUriNode(this string value, string iri) => new NodeFactory().CreateUriNode(value.ToUri(iri));
        public static IUriNode ToUriNode(this int value, string iri) => new NodeFactory().CreateUriNode(value.ToUri(iri));
    }
}
