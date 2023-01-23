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
using BH.oM.Structure.Elements;
using System.Drawing;
using Point = BH.oM.Geometry.Point;
using NUnit.Framework;

namespace BH.Test.RDF
{
    public class ToTTLTests : Test
    {
        [Test]
        public static void BHoMObject_CustomDataAsProperties()
        {
            BHoMObject bhomObject = new BHoMObject();
            bhomObject.CustomData["SomeExtraDataProperty"] = 111;
            bhomObject.CustomData["SomeExtraObjectProperty"] = new CustomObject()
            {
                CustomData = new Dictionary<string, object>
                {
                    {
                        "ANewTypeInstance",
                        new CustomObject()
                        {
                            CustomData = new Dictionary<string, object>()
                            {
                                { "Type", "ANewType" },
                                { "SomeDataProperty", 999 }
                            }
                        }
                    }
                }
            };

            string TTLGraph = Compute.TTLGraph(new List<object>() { bhomObject }, m_ontologySettings);

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CheckURIforBHoMandCustomTypes()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            CustomObject co = new CustomObject();
            co.CustomData[m_ontologySettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<Point> listOfObjects = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            co.CustomData["testListObjects"] = listOfObjects;

            List<object> objectList = new List<object>() { room, co };
            string TTLGraph = objectList.TTLGraph(new OntologySettings()
            {
                TBoxSettings = new TBoxSettings { TreatCustomObjectsWithTypeKeyAsCustomObjectTypes = false, CustomObjectTypesBaseAddress = "www.test.de" },
                ABoxSettings = new ABoxSettings { IndividualsBaseAddress = "https://www.nondefaultURL.com" }
            });

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void EncodeDecode()
        {
            var obj = new KeyValuePair<string, string>("testKey", "testValue");

            string encoded = obj.ToBase64JsonSerialized();

            object decoded = encoded.FromBase64JsonSerialized();

            Assert.IsEqual(obj, decoded);
        }

        [Test]
        public static void NonBHoMObject()
        {
            TestObjectClass nonBHoM = new TestObjectClass();
            List<object> objectList = new List<object>() { nonBHoM };
            string TTLGraph = objectList.TTLGraph(m_ontologySettings, new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void Base64Encoded()
        {
            BHoMObject bhomObj = new BHoMObject();

            //var testEntry = new KeyValuePair<string, string>("testKey", "testValue");
            var testEntry = new TestObjectClass();

            bhomObj.CustomData["encoded"] = testEntry;

            List<object> objectList = new List<object>() { bhomObj };
            string TTLGraph = objectList.TTLGraph(m_ontologySettings);

            Assert.IsTTLParsable(TTLGraph);

            OntologyResource individual = TTLGraph.Individuals().FirstOrDefault();
            string valueString = (individual.TriplesWithSubject.LastOrDefault().Object as LiteralNode)?.Value;

            object decryptedObj = valueString.FromBase64JsonSerialized();

            Dictionary<string, object> decryptedCustomData = decryptedObj as Dictionary<string, object>;

            object decryptedEntry = decryptedCustomData["encoded"];

            Assert.IsEqual(testEntry, decryptedEntry);
        }

        [Test]
        public static void Point()
        {
            Point p = new Point() { X = 101, Y = 102, Z = 103 };
            List<object> objectList = new List<object>() { p };
            string TTLGraph = objectList.TTLGraph(m_ontologySettings);

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            List<object> objectList = new List<object>() { room };
            CSharpGraph CSharpGraph = objectList.CSharpGraph(m_ontologySettings);
            string TTLGraph = CSharpGraph.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void Column()
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            List<object> objectList = new List<object>() { randomColumn };
            string TTLGraph = objectList.TTLGraph(m_ontologySettings);

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void RoomAndColumn()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();

            List<object> objectList = new List<object>() { room, randomColumn };
            string TTLGraph = objectList.TTLGraph(new OntologySettings());

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" },
                { "intProperty", 10 },
                { "pointProperty", BH.Engine.RDF.Testing.Create.RandomObject<Point>() } });

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { customObject }, m_ontologySettings);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }


        [Test]
        public static void CustomObject_SameType_SameProperties_NoError()
        {
            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", 1 } });
            CustomObject customObject2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", 999 } }); // same property assigned to the same "type", only with a different value

            // No error or exception should be thrown by this call.
            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { customObject1, customObject2 }, m_ontologySettings);
            Assert.Single(cSharpGraph_customObj.Classes.Where(c => c.Name == "Cassette"), "CustomObjectTypes");
            Assert.TotalCount(cSharpGraph_customObj.AllIndividuals, 2, "AllIndividuals");

            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomObject_SameType_DifferentProperties_Error()
        {
            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", null } });
            CustomObject customObject2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop2", null } }); // different property assigned to the same "type"

            Assert.ThrowsException(() => Compute.CSharpGraph(new List<object>() { customObject1, customObject2 }, m_ontologySettings));
        }

        [Test]
        public static void NestedCustomObjects()
        {
            CustomObject nestedObj2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "TileMaterial" }, { "MaterialKind", "Stone" } });
            CustomObject nestedObj1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "SimpleTiles" }, { "TileDepth", 0.1 }, { "Material", nestedObj2 } });
            CustomObject parent = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Roof" }, { "Tiles", nestedObj1 } });

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { parent }, m_ontologySettings);

            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "TileMaterial"));
            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "SimpleTiles"));
            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "Roof"));

            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomType_Property_ListOfPrimitives()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_ontologySettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<int> listOPrimitives = Enumerable.Range(0, 10).ToList();
            co.CustomData["testListPrimitives"] = listOPrimitives;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_ontologySettings);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomType_Property_ListOfObjects()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_ontologySettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<Point> listOfObjects = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            co.CustomData["testListObjects"] = listOfObjects;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_ontologySettings);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomType_Property_BoxedListOfObjects()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_ontologySettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";

            // This property is boxed into a System.Object
            List<Point> listOfPoints = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            List<object> boxedProperty = listOfPoints.OfType<object>().ToList();
            co.CustomData["testListObjects"] = boxedProperty;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_ontologySettings);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void NurbsCurve_ControlPoints()
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

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { nurbs }, m_ontologySettings);
            string TTLGraph = cSharpGraph_customObj.ToTTLGraph();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void BHoMObject_Property_ListOfObjects_Boxed()
        {
            BHoMObject bhomObj = new BHoMObject();

            bhomObj.CustomData["boxedListOfObjects"] = new List<object>() {
                    new Point(),
                    new Point() { X = 1, Y = 1, Z = 1 },
                    new Point() { X = 2, Y = 2, Z = 2 }
                };

            string TTLGraph = new List<object>() { bhomObj }.TTLGraph(m_ontologySettings);

            Assert.IsTTLParsable(TTLGraph); // This MUST return an encoded customData dictionary - its entry is not seen as property!
        }
    }
}
