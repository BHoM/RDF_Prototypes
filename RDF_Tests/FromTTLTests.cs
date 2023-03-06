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
using BH.Engine.Adapters.RDF;
using BH.oM.Adapters.RDF;
using BH.oM.Physical.Elements;
using BH.oM.Base;
using VDS.RDF;
using VDS.RDF.Parsing;
using System.Linq;
using System.IO;
using VDS.RDF.Ontology;
using BH.oM.Structure.Elements;
using NUnit.Framework;
using BH.Adapters.TTL;
using Compute = BH.Engine.Adapters.RDF.Compute;
using Convert = BH.Engine.Adapters.TTL.Convert;
using BH.Engine.Adapters.TTL;

namespace BH.Test.RDF
{
    public class FromTTLTests : Test
    {

        [SetUp]
        public void SetUp()
        {
            m_graphSettings = new GraphSettings()
            {
                ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "http://individuals.Address" },
                TBoxSettings = new TBoxSettings() { CustomObjectTypesBaseAddress = "http://CustomObjectTypes.Address" }
            };

            m_adapter = new TTLAdapter(null, m_graphSettings);
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

            CSharpGraph cSharpGraph = Compute.CSharpGraph(new List<object>() { bhomObj }, m_graphSettings);

            string TTLGraph = cSharpGraph.ToTTL();

            Assert.IsTTLParsable(TTLGraph); // This MUST return an encoded customData dictionary - its entry is not seen as property!

            var objs = Engine.Adapters.TTL.Convert.FromTTL(TTLGraph).Item1;

            Assert.IsEqual(bhomObj, objs.First());
        }

        [Test]
        public static void CustomType_Property_BoxedListOfObjects()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";

            // This property is boxed into a System.Object
            List<Point> listOfPoints = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            List<object> boxedProperty = listOfPoints.OfType<object>().ToList();
            co.CustomData["testListObjects"] = boxedProperty;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            var res = Convert.FromTTL(TTLGraph).Item1.First();

            Assert.IsEqual(co, res);
        }

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

            string TTLGraph = Engine.Adapters.TTL.Compute.ToTTL(new List<object>() { bhomObject }, m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();

            Assert.IsEqual(bhomObject, bhomObjects.FirstOrDefault());
        }

        [Test]
        public static void CheckWritingOfTBoxABoxSettings()
        {
            Room room = new Room();

            CustomObject co = new CustomObject();
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<Point> listOfObjects = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            co.CustomData["testListObjects"] = listOfObjects;

            List<object> objectList = new List<object>() { room, co };

            // Create ontology settings with TBoxSettings and ABoxSettings all having values different than the default values.
            var customOntologySettings = new GraphSettings()
            {
                TBoxSettings = new TBoxSettings()
                {
                    TreatCustomObjectsWithTypeKeyAsCustomObjectTypes = !new TBoxSettings().TreatCustomObjectsWithTypeKeyAsCustomObjectTypes,
                    CustomobjectsTypeKey = "SomethingNotType",
                    TypeUris = new Dictionary<Type, string>()
                    {
                        { typeof(Room), "http://someUri.com#room" },
                        { typeof(CustomObject), "http://someotherUri.com#customType" }
                    },
                    CustomObjectTypesBaseAddress = "http://customObjects.gnappo",
                    DefaultBaseUriForUnknownTypes = "http://unkownTypes.gnappo"
                },
                ABoxSettings = new ABoxSettings()
                {
                    IndividualsBaseAddress = "http://individualBaseAddress.gnappo",
                    ConsiderNullOrEmptyPropertyValues = !new ABoxSettings().ConsiderNullOrEmptyPropertyValues
                }
            };

            string TTLGraph = objectList.ToTTL(customOntologySettings);

            Assert.IsTTLParsable(TTLGraph);

            Output<List<object>, GraphSettings> bhomObjects_ontologySettings = BH.Engine.Adapters.TTL.Convert.FromTTL(TTLGraph);

            Assert.IsEqual(bhomObjects_ontologySettings.Item2, customOntologySettings);
        }

        [Test]
        public static void Point()
        {
            // This test checks a geometry, so we set this to true.
            m_graphSettings.ABoxSettings.GeometryAsOntologyClass = true;

            Point p = new Point() { X = 101, Y = 102, Z = 103 };
            List<object> objectList = new List<object>() { p };

            CSharpGraph cSharpGraph = Compute.CSharpGraph(objectList, m_graphSettings);

            string TTLGraph = cSharpGraph.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();

            Assert.IsEqual(p, bhomObjects.FirstOrDefault());
        }

        [Test]
        public static void Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 102, Z = 103 }, new Point() { X = 201, Y = 202, Z = 203 }, new Point() { X = 301, Y = 302, Z = 303 } } };
            room.Location = new Point() { X = 401, Y = 402, Z = 403 };
            room.Name = "A room object";

            List<object> objectList = new List<object>() { room };
            string TTLGraph = objectList.ToTTL(m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();
            Assert.IsEqual(room, bhomObjects.FirstOrDefault());
        }

        [Test]
        public static void Column()
        {
            Column randomColumn = CreateRandomColumn();

            List<object> objectList = new List<object>() { randomColumn };
            string TTLGraph = objectList.ToTTL(m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();
            Assert.IsEqual(randomColumn, bhomObjects.FirstOrDefault());
        }

        [Test]
        public static void RoomAndColumn()
        {
            Room room = new Room();
            // Note the duplicate values for Y,Z in the first/second points of ControlPoints.
            // The duplicate removal mechanism of the CSharpGraph should NOT identify these values as duplicates, as they are used in different points.
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 202, Z = 202 }, new Point() { X = 201, Y = 202, Z = 202 }, new Point() { X = 301, Y = 302, Z = 303 } } };
            room.Location = new Point() { X = 401, Y = 402, Z = 403 };
            room.Name = "A room object";

            Column column = CreateRandomColumn();

            List<object> objectList = new List<object>() { room, column };
            string TTLGraph = objectList.ToTTL(m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();
            Assert.IsEqual(objectList, bhomObjects);

            bhomObjects[1] = new Column();
            Assert.IsNotEqual(objectList, bhomObjects);
        }

        [Test]
        public static void CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" },
                { "intProperty", 10 },
                { "pointProperty", BH.Engine.Adapters.RDF.Testing.Create.RandomObject<Point>() } });

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { customObject }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomObject_MeshProperty()
        {

            BH.oM.Geometry.Mesh mesh = new Mesh()
            {
                Faces = new List<Face>() { new Face() { A = 101, B = 102, C = 103, D = 104 }, new Face() { A = 201, B = 202, C = 203, D = 204 } },
                Vertices = new List<Point>() { new Point() { X = 801, Y = 802, Z = 803 }, new Point() { X = 901, Y = 902, Z = 903 } }
            };

            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Roof" }, { "RoofShape", mesh } });

            // No error or exception should be thrown by this call.
            string TTLGraph = new List<object>() { customObject1 }.ToTTL(m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);

            Output<List<object>, GraphSettings> convertedObjs = BH.Engine.Adapters.TTL.Convert.FromTTL(TTLGraph);

            Assert.IsEqual(customObject1, convertedObjs.Item1.First());
        }

        [Test]
        public static void CustomObject_SameType_SameProperties_NoError()
        {
            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", 1 } });
            CustomObject customObject2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", 999 } }); // same property assigned to the same "type", only with a different value

            // No error or exception should be thrown by this call.
            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { customObject1, customObject2 }, m_graphSettings);
            Assert.Single(cSharpGraph_customObj.Classes.Where(c => c.Name == "Cassette"), "CustomObjectTypes");
            Assert.TotalCount(cSharpGraph_customObj.AllIndividuals, 2, "AllIndividuals");

            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomObject_SameType_DifferentProperties_Error()
        {
            CustomObject customObject1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop1", null } });
            CustomObject customObject2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" }, { "Prop2", null } }); // different property assigned to the same "type"

            Assert.ThrowsException(() => Compute.CSharpGraph(new List<object>() { customObject1, customObject2 }, m_graphSettings));
        }

        [Test]
        public static void NestedCustomObjects()
        {
            CustomObject nestedObj2 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "TileMaterial" }, { "MaterialKind", "Stone" } });
            CustomObject nestedObj1 = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "SimpleTiles" }, { "TileDepth", 0.1 }, { "Material", nestedObj2 } });
            CustomObject parent = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Roof" }, { "Tiles", nestedObj1 } });

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { parent }, m_graphSettings);

            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "TileMaterial"));
            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "SimpleTiles"));
            Assert.IsNotNull(cSharpGraph_customObj.Classes.Single(c => c.Name == "Roof"));

            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();

            Assert.IsEqual(parent, bhomObjects.FirstOrDefault());
        }

        [Test]
        public static void NurbsCurve_PropertyList_OfObjects()
        {
            // This test checks a geometry, so we set this to true.
            m_graphSettings.ABoxSettings.GeometryAsOntologyClass = true;

            NurbsCurve nurbs = new NurbsCurve()
            {
                ControlPoints = new List<Point>()
                {
                    new Point() { X = 101, Y = 102, Z = 103 },
                    new Point() { X = 201, Y = 202, Z = 203 },
                    new Point() { X = 301, Y = 302, Z = 303 }
                }
            };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { nurbs }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            var bhomObjects = TTLGraph.ToCSharpObjects();

            Assert.IsEqual(nurbs, bhomObjects.FirstOrDefault());
        }

        [Test]
        public static void BHoMObject_CustomData_PropertyList_OfPrimitives()
        {
            BHoMObject bhomObj = new BHoMObject();
            bhomObj.CustomData["listOfPrimitives"] = new List<int>() { 1, 2, 3, 4 };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { bhomObj }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            var convertedObj = TTLGraph.ToCSharpObjects().FirstOrDefault();

            Assert.IsEqual(bhomObj.CustomData["listOfPrimitives"], (convertedObj as BHoMObject)?.CustomData["listOfPrimitives"]);
        }

        [Test]
        public static void FEMeshFace_PropertyList_OfPrimitives()
        {
            // This test checks a geometry, so we set this to true.
            m_graphSettings.ABoxSettings.GeometryAsOntologyClass = true;

            BH.oM.Structure.Elements.FEMeshFace bhomObj = new BH.oM.Structure.Elements.FEMeshFace();
            bhomObj.NodeListIndices = new List<int>() { 1, 2, 3, 4 };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { bhomObj }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            var convertedObj = TTLGraph.ToCSharpObjects().FirstOrDefault() as BH.oM.Structure.Elements.FEMeshFace;

            Assert.IsEqual(bhomObj.NodeListIndices, convertedObj.NodeListIndices);
        }

        [Test]
        public static void CustomType_PropertyList_OfPrimitives()
        {
            CustomObject customObj = new CustomObject();
            customObj.CustomData["listOfPrimitives"] = new List<int>() { 1, 2, 3, 4 };

            string TTLGraph = new List<object>() { customObj }.ToTTL(m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);

            var convertedObj = TTLGraph.ToCSharpObjects().FirstOrDefault() as CustomObject;

            Assert.IsEqual(customObj.CustomData["listOfPrimitives"], convertedObj.CustomData["listOfPrimitives"]);
        }

        [Test]
        public static void NonBHoMObject()
        {
            TestObjectClass nonBHoM = new TestObjectClass();
            List<object> objectList = new List<object>() { nonBHoM };
            string TTLGraph = objectList.ToTTL(m_graphSettings, new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);

            var convertedObj = TTLGraph.ToCSharpObjects().FirstOrDefault();

            Assert.IsEqual(nonBHoM, convertedObj);
        }
    }
}
