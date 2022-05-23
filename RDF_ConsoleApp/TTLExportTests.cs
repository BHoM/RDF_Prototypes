using BH.oM.Architecture.Elements;
using System;
using System.Collections.Generic;
using BH.oM.Geometry;
using BH.Engine.RDF;
using BH.oM.RDF;
using BH.oM.Physical.Elements;
using BH.oM.Base;

namespace BH.Test.RDF
{
    public static class TTLExportTests
    {
        private static OntologySettings m_shortAddresses = new OntologySettings()
        {
            ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "individualsAddress" },
            TBoxSettings = new TBoxSettings() { CustomTypesBaseAddress = "customTypesAddress" }
        };

        public static string Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            List<IObject> objectList = new List<IObject>() { room };
            string TTL = objectList.TTLGraph(new OntologySettings(), new LocalRepositorySettings());

            return TTL;
        }

        public static void Column()
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            List<IObject> objectList = new List<IObject>() { randomColumn };
            string TTLGraph = objectList.TTLGraph(new OntologySettings(), new LocalRepositorySettings());
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

            return TTLGraph;
        }

        public static string CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" },
                { "intProperty", 10 },
                { "pointProperty", BH.Engine.RDF.Testing.Create.RandomObject<Point>() } });
            CSharpGraph cSharpGraph_customObj = customObject.CSharpGraph(m_shortAddresses);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph(new LocalRepositorySettings());

            return TTLGraph;
        }
    }
}
