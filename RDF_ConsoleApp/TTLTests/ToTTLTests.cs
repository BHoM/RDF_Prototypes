using BH.oM.Architecture.Elements;
using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.Engine.RDF;
using BH.oM.RDF;
using BH.oM.Physical.Elements;
using BH.oM.Base;
using VDS.RDF;
using VDS.RDF.Parsing;
using System.Linq;
using System.IO;
using VDS.RDF.Ontology;

namespace BH.Test.RDF
{
    public static class ToTTLTests
    {
        public static void EncodeDecode()
        {
            var obj = new KeyValuePair<string, string>("testKey", "testValue");

            string encoded = obj.ToBase64JsonSerialized();

            object decoded = encoded.FromBase64JsonSerialized();

            Assert.IsEqual(obj, decoded);
        }

        public class TestClass
        {
            public string Name { get; set; } = "TestName";
            public int Int { get; set; } = 999;
        }

        public static void Base64Encoded()
        {
            BHoMObject bhomObj = new BHoMObject();

            //var testEntry = new KeyValuePair<string, string>("testKey", "testValue");
            var testEntry = new TestClass();
            
            bhomObj.CustomData["encoded"] = testEntry;

            List<IObject> objectList = new List<IObject>() { bhomObj };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            OntologyResource individual = TTLGraph.Individuals().FirstOrDefault();
            string valueString = (individual.TriplesWithSubject.LastOrDefault().Object as LiteralNode)?.Value;

            object decryptedObj = valueString.FromBase64JsonSerialized();

            Dictionary<string, object> decryptedCustomData = decryptedObj as Dictionary<string, object>;

            object decryptedEntry = decryptedCustomData["encoded"];

            Assert.IsEqual(testEntry, decryptedEntry);
        }

        public static void Point()
        {
            Point p = new Point() { X = 101, Y = 102, Z = 103 };
            List<IObject> objectList = new List<IObject>() { p };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            List<OntologyResource> individuals = TTLGraph.Individuals();

            var bhomObjects = TTLGraph.ToCSharpObjects();
        }

        public static string Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            List<IObject> objectList = new List<IObject>() { room };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            return TTLGraph;
        }

        public static string Column()
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            List<IObject> objectList = new List<IObject>() { randomColumn };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            Console.Write(TTLGraph);

            Assert.IsTTLParsable(TTLGraph);

            return TTLGraph;
        }

        public static string RoomAndColumn()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            List<IObject> objectList = new List<IObject>() { room, randomColumn };
            string TTLGraph = objectList.TTLGraph(new OntologySettings(), new LocalRepositorySettings());

            Console.Write(TTLGraph);

            Assert.IsTTLParsable(TTLGraph);

            return TTLGraph;
        }

        public static string CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" },
                { "intProperty", 10 },
                { "pointProperty", BH.Engine.RDF.Testing.Create.RandomObject<Point>() } });

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { customObject }, m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Console.Write(TTLGraph);

            Assert.IsTTLParsable(TTLGraph);

            return TTLGraph;
        }


        public static string CustomObject_SameType_SameProperties_NoError()
        {
            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", 1 } });
            CustomObject customObject2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", 999 } }); // same property assigned to the same "type", only with a different value

            // No error or exception should be thrown by this call.
            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { customObject1, customObject2 }, m_shortAddresses);
            Assert.Single(cSharpGraph_customObj.Classes.Where(c => c.Name == "Cassette"), "CustomObjectTypes");
            Assert.TotalCount(cSharpGraph_customObj.AllIndividuals, 2, "AllIndividuals");

            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Console.Write(TTLGraph);

            Assert.IsTTLParsable(TTLGraph);

            return TTLGraph;
        }

        public static void CustomObject_SameType_DifferentProperties_Error()
        {
            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", null } });
            CustomObject customObject2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop2", null } }); // different property assigned to the same "type"

            Assert.ThrowsException(() => Compute.CSharpGraph(new List<IObject>() { customObject1, customObject2 }, m_shortAddresses));
        }

        public static void NestedCustomObjects()
        {
            CustomObject nestedObj2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "TileMaterial" }, { "MaterialKind", "Stone" } });
            CustomObject nestedObj1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "SimpleTiles" }, { "TileDepth", 0.1 }, { "Material", nestedObj2 } });
            CustomObject parent = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Roof" }, { "Tiles", nestedObj1 } });

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { parent }, m_shortAddresses);

            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "TileMaterial"));
            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "SimpleTiles"));
            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "Roof"));

            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);
        }

        public static void CustomType_Property_ListOfPrimitives()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_shortAddresses.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<int> listOPrimitives = Enumerable.Range(0, 10).ToList();
            co.CustomData["testListPrimitives"] = listOPrimitives;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { co }, m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);
        }

        public static void CustomType_Property_ListOfObjects()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_shortAddresses.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<Point> listOfObjects = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            co.CustomData["testListObjects"] = listOfObjects;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { co }, m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);
        }

        public static string BHoMObject_Property_ListOfObjects()
        {
            NurbsCurve nurbs = new NurbsCurve()
            {
                ControlPoints = new List<Point>()
                {
                    new Point(),
                    new Point() { X = 1, Y = 1, Z = 1 },
                    new Point() { X = 2, Y = 2, Z = 2 }
                }
            };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { nurbs }, m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            return TTLGraph;
        }

        // ----------------------
        // Run utilities
        // ----------------------

        public static void RunAll()
        {
            // Invoke all static methods in the given class class
            typeof(ToTTLTests).GetMethods()
                .Where(mi => mi.IsStatic && !mi.Name.Contains("Run")).ToList()
                .ForEach(mi => mi.Invoke(null, null));

            Assert.TestRecap();
        }

        public static void RunSelectedTests()
        {
            CustomType_Property_ListOfPrimitives();

            CustomType_Property_ListOfObjects();

            BHoMObject_Property_ListOfObjects();

            EncodeDecode();

            Base64Encoded();

            Point();

            NestedCustomObjects();

            CustomObject_SameType_SameProperties_NoError();

            CustomObject_SameType_DifferentProperties_Error();

            Room();

            RoomAndColumn();

            CustomObject();

        }




        // ----------------------
        // Private fields
        // ----------------------

        private static OntologySettings m_shortAddresses = new OntologySettings()
        {
            ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "individuals.Address" },
            TBoxSettings = new TBoxSettings() { CustomObjectTypesBaseAddress = "CustomObjectTypes.Address" }
        };
    }
}
