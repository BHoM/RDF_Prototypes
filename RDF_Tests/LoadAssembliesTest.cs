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
using BH.oM.Physical.FramingProperties;
using VDS.RDF.Query.Expressions.Comparison;
using FluentAssertions;
using BH.Adapters.TTL;
using Compute = BH.Engine.RDF.Compute;
using BH.Engine.Adapters.TTL;
using Shouldly;

namespace BH.Test.RDF
{
    public class LoadAssembliesTest : Test
    {
        private static System.Reflection.Assembly[] assembliesLoaded = null;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            BH.Engine.RDF.Compute.LoadAssembliesInDirectory(@"C:\ProgramData\BHoM\Assemblies", onlyBHoMAssemblies:true, onlyoMAssemblies:false);
            assembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();
        }

        [Test]
        public static void LoadAssemblies_RDF()
        {
            var names = assembliesLoaded.Select(a => a.FullName).ToList();
            var n1 = names.Where(n => n.Contains("RDF_Engine")).ToList();
            n1.ShouldHaveSingleItem();
            names.Where(n => n.Contains("RDF_oM")).ShouldHaveSingleItem();
        }

        [Test]
        public static void LoadAssemblies_TTL()
        {
            var names = assembliesLoaded.Select(a => a.FullName).ToList();
            names.Where(n => n.Contains("TTL_Engine")).ShouldHaveSingleItem();
            names.Where(n => n.Contains("TTL_Adapter")).ShouldHaveSingleItem();
        }
    }
}
