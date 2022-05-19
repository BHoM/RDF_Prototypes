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
        public static string Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            OntologySettings ontologySett = new OntologySettings();
            LocalRepositorySettings localRepositorySett = new LocalRepositorySettings();

            string TTL = room.TTLGraph(ontologySett, localRepositorySett);
            return TTL;
        }

        public static void Column()
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();
            CSharpGraph cSharpGraph = Compute.CSharpGraph(randomColumn, new OntologySettings());
            LocalRepositorySettings diellzasettings = new LocalRepositorySettings()
            {
                RepositoryRootPath = @"C:\Users\diels\source"
            };
            string TTLGraph = randomColumn.TTLGraph(new OntologySettings(), diellzasettings);
        }

        public static void CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "firstProperty", 10 }, { "secondProperty", 20 } });
            CSharpGraph cSharpGraph_customObj = customObject.CSharpGraph(new OntologySettings());
        }
    }
}
