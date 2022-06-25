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
    public static class FromTTLTests
    {
        public static void Point()
        {
            Point p = new Point() { X = 101, Y = 102, Z = 103 };
            List<IObject> objectList = new List<IObject>() { p };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            var bhomObjects = TTLGraph.ToBHoMInstances();

            Assert.IsEqual(p, bhomObjects.FirstOrDefault());
        }

        public static void Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 102, Z = 103 }, new Point() { X = 201, Y = 202, Z = 203 }, new Point() { X = 99 } } };
            room.Location = new Point() { X = 901, Y = 902, Z = 903 };
            room.Name = "A room object";

            List<IObject> objectList = new List<IObject>() { room };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            var bhomObjects = TTLGraph.ToBHoMInstances();
            Assert.IsEqual(room, bhomObjects.FirstOrDefault());
        }

        public static void Column()
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            List<IObject> objectList = new List<IObject>() { randomColumn };
            string TTLGraph = objectList.TTLGraph(m_shortAddresses, new LocalRepositorySettings());

            Console.Write(TTLGraph);

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToBHoMInstances();
            Assert.IsEqual(randomColumn, bhomObjects.FirstOrDefault());
        }

        public static void RoomAndColumn()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point() { X = 101, Y = 102, Z = 103 };
            room.Name = "A room object";

            Column column = new Column();
            column.Location = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 102, Z = 103 }, new Point() { X = 201, Y = 202, Z = 203 } } };
            column.Name = "A column object";

            List<IObject> objectList = new List<IObject>() { room, column };
            string TTLGraph = objectList.TTLGraph(new OntologySettings(), new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToBHoMInstances();
            Assert.IsEqual(objectList, bhomObjects);

            bhomObjects[1] = new Column();
            Assert.IsNotEqual(objectList, bhomObjects);
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

            Console.Write(TTLGraph);

            Assert.IsTTLParsable(TTLGraph);
        }

        public static void IObject_PropertyList_OfObjects()
        {
            NurbsCurve nurbs = new NurbsCurve()
            {
                ControlPoints = new List<Point>()
                {
                    new Point() { X = 101, Y = 102, Z = 103 },
                    new Point() { X = 201, Y = 202, Z = 203 },
                    new Point() { X = 301, Y = 302, Z = 303 }
                }
            };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { nurbs }, m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);
            
            var bhomObjects = TTLGraph.ToBHoMInstances();

            Assert.IsEqual(nurbs, bhomObjects.FirstOrDefault());
        }

        public static void IObject_PropertyList_OfPrimitives()
        {
            BHoMObject bhomObj = new BHoMObject();
            bhomObj.CustomData["listOfPrimitives"] = new List<int>() { 1, 2, 3, 4 };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<IObject>() { bhomObj }, m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            var convertedObj = TTLGraph.ToBHoMInstances().FirstOrDefault() as BHoMObject;

            Assert.IsEqual(bhomObj.CustomData["listOfPrimitives"], convertedObj.CustomData["listOfPrimitives"]);
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
            RoomAndColumn();

            CustomObject();

            IObject_PropertyList_OfObjects();

            IObject_PropertyList_OfPrimitives();

            Point();

            Room();

            NestedCustomObjects();

            CustomObject_SameType_SameProperties_NoError();

            CustomObject_SameType_DifferentProperties_Error();
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
