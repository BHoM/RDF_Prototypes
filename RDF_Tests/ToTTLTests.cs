/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Drawing;
using Point = BH.oM.Geometry.Point;
using NUnit.Framework;
using BH.oM.Physical.FramingProperties;
using VDS.RDF.Query.Expressions.Comparison;
using FluentAssertions;
using BH.Adapters.TTL;
using Compute = BH.Engine.Adapters.RDF.Compute;
using BH.Engine.Adapters.TTL;
using BH.Engine.Base;
using Convert = BH.Engine.Adapters.TTL.Convert;
using BH.Engine.Adapters.TTL;
using System.Text.RegularExpressions;
using Shouldly;
using System.Linq.Expressions;
using System.Drawing.Printing;
using static System.Net.WebRequestMethods;

namespace BH.Test.RDF
{
    public class ToTTLTests : TestClass
    {
        [SetUp]
        public void SetUp()
        {
            m_graphSettings = new GraphSettings()
            {
                ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "http://individuals.Address" },
                TBoxSettings = new TBoxSettings() { CustomObjectTypesBaseAddress = "http://CustomObjectTypes.Address" }
            };
        }

        [Test]
        public static void Geometry_Serialized()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            CSharpGraph cSharpGraph = Compute.CSharpGraph(new List<object>() { room }, new GraphSettings()
            {
                TBoxSettings = new TBoxSettings { GeometryAsOntologyClass = false }
            });

            string TTLGraph = new List<object>() { room }.ToTTL(m_graphSettings);

            Assert.IsTrue(!cSharpGraph.ObjectProperties.Any() && !TTLGraph.Contains(room.Location.ToString()));

        }

        [Test]
        public static void Geometry_Non_Serialized()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            CSharpGraph cSharpGraph = Compute.CSharpGraph(new List<object>() { room }, new GraphSettings()
            {
                TBoxSettings = new TBoxSettings { GeometryAsOntologyClass = true }
            });

            Assert.IsTrue(cSharpGraph.Classes.Contains(room.Location.GetType()));


            string TTLGraph = new List<object>() { room }.ToTTL(new GraphSettings()
            {
                TBoxSettings = new TBoxSettings { GeometryAsOntologyClass = true }
            });

            Assert.IsTrue(cSharpGraph.ObjectProperties.Any() && TTLGraph.Contains(room.Location.ToString()));

        }

        [Test]
        public static void GeometryAsBase64()
        {
            m_graphSettings.TBoxSettings.GeometryAsOntologyClass = false;

            Room room = new Room() { Perimeter = new Line(), Location = new Point() };

            CSharpGraph cSharpGraph = Compute.CSharpGraph(new List<object> { room }, m_graphSettings);

            // Because all the properties of Room are not Ontology types (they are geometrical types),
            // the cSharpGraph must not have any ObjectProperty.
            Assert.IsEqual(cSharpGraph.ObjectProperties.Any(), false);

            Assert.IsEqual(cSharpGraph.DataProperties.Any(), true);

            string ttl = cSharpGraph.ToTTL();

            Assert.IsTTLParsable(ttl);
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

            string TTLGraph = Engine.Adapters.TTL.Convert.ToTTL(new List<object>() { bhomObject }, m_graphSettings);

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
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<Point> listOfObjects = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            co.CustomData["testListObjects"] = listOfObjects;

            List<object> objectList = new List<object>() { room, co };
            string TTLGraph = objectList.ToTTL(new GraphSettings()
            {
                TBoxSettings = new TBoxSettings { TreatCustomObjectsWithTypeKeyAsCustomObjectTypes = false, CustomObjectTypesBaseAddress = "http://www.test.de" },
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
            string TTLGraph = objectList.ToTTL(m_graphSettings, new LocalRepositorySettings());

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void Point()
        {
            m_graphSettings.TBoxSettings.GeometryAsOntologyClass = true;

            Point p = new Point() { X = 101, Y = 102, Z = 103 };
            List<object> objectList = new List<object>() { p };
            string TTLGraph = objectList.ToTTL(m_graphSettings);

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
            CSharpGraph CSharpGraph = objectList.CSharpGraph(m_graphSettings);
            string TTLGraph = CSharpGraph.ToTTL();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void Column()
        {
            Column randomColumn = CreateRandomColumn();

            List<object> objectList = new List<object>() { randomColumn };
            string TTLGraph = objectList.ToTTL(m_graphSettings);

            Assert.IsTTLParsable(TTLGraph);
        }



        [Test]
        public static void RoomAndColumn()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point();
            room.Name = "A room object";

            Column randomColumn = CreateRandomColumn();

            List<object> objectList = new List<object>() { room, randomColumn };
            string TTLGraph = objectList.ToTTL(new GraphSettings());

            Assert.IsTTLParsable(TTLGraph);
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
        }

        [Test]
        public static void CustomType_Property_ListOfPrimitives()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<int> listOPrimitives = Enumerable.Range(0, 10).ToList();
            co.CustomData["testListPrimitives"] = listOPrimitives;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void CustomType_Property_ListOfObjects()
        {
            CustomObject co = new CustomObject();
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            List<Point> listOfObjects = new List<Point>() { new oM.Geometry.Point() { X = 101, Y = 102 }, new Point() { X = 201, Y = 202 } };
            co.CustomData["testListObjects"] = listOfObjects;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

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

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { nurbs }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void IndividualsDuplicatedData()
        {
            // Targets https://github.com/BHoM/RDF_Prototypes/issues/109

            CustomObject testObj = new CustomObject();

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { testObj }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);

            // Check how many times the BHoM_Guid property appears in the TTL text.
            string guidIndividualProperty = $".BHoM_Guid \"{testObj.BHoM_Guid}\"^^xsd:string";
            int repetitionOfGuidProperty = TTLGraph.CountSubstring(guidIndividualProperty);

            // Verify that the number of repetitions is equal to 1.
            repetitionOfGuidProperty.ShouldBe(1); // this fails.
        }

        [Test]
        public static void List_polycurveObj()
        {
            PolyCurve polyCurve = new PolyCurve()
            {
                Curves = new List<ICurve>
                {
                    new Line() { Start = new Point() { X = 1, Y = 1, Z = 0 }, End = new Point() { X = 2, Y = 1, Z = 0 } },
                    new Line() { Start = new Point() { X = 2, Y = 1, Z = 0 }, End = new Point() { X = 3, Y = 1, Z = 0 } }
                }
            };

            GraphSettings settings = new GraphSettings() { TBoxSettings = new TBoxSettings() { GeometryAsOntologyClass = true }, ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "https://uni-stuttgart.de/" } };

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { polyCurve }, settings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void ListOfNumbers()
        {
            List<int> numbers = new List<int>();
            numbers.Add(1);
            numbers.Add(2);
            numbers.Add(3);

            CustomObject co = new CustomObject();
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            co.CustomData["testListNumbers"] = numbers;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            //TTLGraph.Contains(co.BHoM_Guid.ToString() + m_graphSettings.ABoxSettings.SequenceIndentifierSuffix).ShouldBeTrue();
            TTLGraph.Contains($"rdf:_1 \"{numbers[0]}\"^^xsd:integer;").ShouldBeTrue();
            TTLGraph.Contains($"rdf:_2 \"{numbers[1]}\"^^xsd:integer;").ShouldBeTrue();
            TTLGraph.Contains($"rdf:_3 \"{numbers[2]}\"^^xsd:integer.").ShouldBeTrue();

            Assert.IsTTLParsable(TTLGraph);
        }

        [Test]
        public static void ListOfStrings()
        {
            List<string> strings = new List<string>();
            strings.Add("Apple");
            strings.Add("Banana");
            strings.Add("Cherry");

            CustomObject co = new CustomObject();
            co.CustomData[m_graphSettings.TBoxSettings.CustomobjectsTypeKey] = "TestType";
            co.CustomData["testListStrings"] = strings;

            CSharpGraph cSharpGraph_customObj = Compute.CSharpGraph(new List<object>() { co }, m_graphSettings);
            string TTLGraph = cSharpGraph_customObj.ToTTL();

            //TTLGraph.Contains(co.BHoM_Guid.ToString() + m_graphSettings.ABoxSettings.SequenceIndentifierSuffix).ShouldBeTrue();
            TTLGraph.Contains($"rdf:_1 \"{strings[0]}\"^^xsd:string;").ShouldBeTrue();
            TTLGraph.Contains($"rdf:_2 \"{strings[1]}\"^^xsd:string;").ShouldBeTrue();
            TTLGraph.Contains($"rdf:_3 \"{strings[2]}\"^^xsd:string.").ShouldBeTrue();

            Assert.IsTTLParsable(TTLGraph);
        }
    }
}

