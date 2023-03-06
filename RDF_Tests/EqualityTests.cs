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
using BH.Engine.Base;
using NUnit.Framework;

namespace BH.Test.RDF
{
    public static class EqualityTests
    {
        [Test]
        public static void Point()
        {
            Point p = new Point() { X = 101, Y = 102, Z = 103 };

            Assert.IsNotEqual(p, new Point());
            Assert.IsEqual(p, p.DeepClone());
        }

        [Test]
        public static void Room()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 102, Z = 103 }, new Point() { X = 201, Y = 202, Z = 203 }, new Point() { X = 99 } } };
            room.Location = new Point() { X = 901, Y = 902, Z = 903 };
            room.Name = "A room object";

            Assert.IsNotEqual(room, new Room());
            Assert.IsEqual(room, room.DeepClone());
        }

        [Test]
        public static void Column()
        {
            Column randomColumn = BH.Engine.Adapters.RDF.Testing.Create.RandomObject<Column>();

            Assert.IsNotEqual(randomColumn, new Column());
            Assert.IsEqual(randomColumn, randomColumn.DeepClone());
        }

        [Test]
        public static void RoomAndColumn()
        {
            Room room = new Room();
            room.Perimeter = new Polyline() { ControlPoints = new List<Point>() { new Point(), new Point() { X = 5, Y = 5, Z = 5 }, new Point() { X = 99 } } };
            room.Location = new Point() { X = 101, Y = 102, Z = 103 };
            room.Name = "A room object";

            Column column = new Column();
            column.Location = new Polyline() { ControlPoints = new List<Point>() { new Point() { X = 101, Y = 102, Z = 103 }, new Point() { X = 201, Y = 202, Z = 203 } } };
            column.Name = "A column object";

            List<object> objectList = new List<object>() { room, column };

            var clonedList = objectList.DeepClone();

            Assert.IsEqual(objectList, clonedList);

            clonedList[1] = new Column();
            Assert.IsNotEqual(objectList, clonedList);
        }

        [Test]
        public static void CustomObject()
        {
            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "Type", "Cassette" },
                { "intProperty", 10 },
                { "pointProperty", BH.Engine.Adapters.RDF.Testing.Create.RandomObject<Point>() } });

            Assert.IsEqual(customObject, customObject.DeepClone());
            Assert.IsNotEqual(customObject, new CustomObject());
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

            var clone = customObject1.DeepClone();

            Assert.IsEqual(customObject1, clone);

            clone.CustomData["RoofShape"] = new Mesh();
            Assert.IsNotEqual(customObject1, clone);
        }

        [Test]
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

        private static GraphSettings m_graphSettings = new GraphSettings()
        {
            ABoxSettings = new ABoxSettings() { IndividualsBaseAddress = "individuals.Address" },
            TBoxSettings = new TBoxSettings() { CustomObjectTypesBaseAddress = "CustomObjectTypes.Address" }
        };
    }
}
