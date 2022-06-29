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
using BH.Engine.Base;

namespace BH.Test.RDF
{
    public static class EqualityTests
    {
        public static void Point()
        {
            Point p = new Point() { X = 101, Y = 102, Z = 103 };

            Assert.IsNotEqual(p, new Point());
            Assert.IsEqual(p, p.DeepClone());
        }

        public static void Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 102, Z = 103 }, new Point() { X = 201, Y = 202, Z = 203 }, new Point() { X = 99 } } };
            room.Location = new Point() { X = 901, Y = 902, Z = 903 };
            room.Name = "A room object";

            Assert.IsNotEqual(room, new Room());
            Assert.IsEqual(room, room.DeepClone());
        }

        public static void Column()
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            Assert.IsNotEqual(randomColumn, new Column());
            Assert.IsEqual(randomColumn, randomColumn.DeepClone());
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

            var clonedList = objectList.DeepClone();

            Assert.IsEqual(objectList, clonedList);

            clonedList[1] = new Column();
            Assert.IsNotEqual(objectList, clonedList);
        }

        public static void CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" },
                { "intProperty", 10 },
                { "pointProperty", BH.Engine.RDF.Testing.Create.RandomObject<Point>() } });

            Assert.IsEqual(customObject, customObject.DeepClone());
            Assert.IsNotEqual(customObject, new CustomObject());
        }

        public static void CustomObject_MeshProperty()
        {
            BH.oM.Geometry.Mesh mesh = new Mesh()
            {
                Faces = new List<Face>() { new Face() { A = 101, B = 102, C = 103, D = 104 }, new Face() { A = 201, B = 202, C = 203, D = 204 } },
                Vertices = new List<Point>() { new Point() { X = 801, Y = 802, Z = 803 }, new Point() { X = 901, Y = 902, Z = 903 } }
            };

            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Roof" }, { "RoofShape", mesh } });

            var clone = customObject1.DeepClone();

            Assert.IsEqual(customObject1, clone);

            clone.CustomData["RoofShape"] = new Mesh();
            Assert.IsNotEqual(customObject1, clone);
        }

        public static void CustomType_PropertyList_OfPrimitives()
        {
            CustomObject customObj = new CustomObject();
            customObj.CustomData["Type"] = "SomeType";
            customObj.CustomData["listOfPrimitives"] = new List<int>() { 1, 2, 3, 4 };

            var clone = customObj.DeepClone();
            Assert.IsEqual(customObj, clone);

            clone.CustomData["listOfPrimitives"] = new List<int>();
            Assert.IsNotEqual(customObj, clone);
        }

        // ----------------------
        // Run utilities
        // ----------------------

        public static void RunAll()
        {
            // Invoke all static methods in the given class class
            typeof(FromTTLTests).GetMethods()
                .Where(mi => mi.IsStatic && !mi.Name.Contains("Run")).ToList()
                .ForEach(mi => { m_ontologySettings.OntologyTitle = mi.Name; mi.Invoke(null, null); });

            Assert.TestRecap();
        }

        public static void RunSelectedTests()
        {
            RoomAndColumn();

            CustomObject();

            Point();

            Room();
        }


        // ----------------------
        // Private fields
        // ----------------------

        private static OntologySettings m_ontologySettings = new OntologySettings()
        {
            ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "individuals.Address" },
            TBoxSettings = new TBoxSettings() { CustomObjectTypesBaseAddress = "CustomObjectTypes.Address" }
        };
    }
}
