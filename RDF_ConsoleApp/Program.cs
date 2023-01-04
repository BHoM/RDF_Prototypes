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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using BH.Engine.RDF;
using BH.oM.RDF;
using BH.Test.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Update;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args = null)
        {
            // Aaron's method invokation
            //Compute.PostToRepo(@"G:\My Drive\03_Arbeit\02_HIWI Stellen\03_Architectural_Computing\TTL Files\repo-config.ttl", @"G:\My Drive\03_Arbeit\02_HIWI Stellen\03_Architectural_Computing\TTL Files\BhomOuput.ttl");
            //Compute.StartGraphDB();

            ToTTLTests.BHoMObject_CustomDataAsProperties();

            EqualityTests.RunAll();

            ToTTLTests.RunAll();

            FromTTLTests.RunAll();

            // Invoke all static methods in `Tests_Diellza` class
            //typeof(Tests_Diellza).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));
            Console.WriteLine("\n\nPress ENTER to repeat, any other key to exit.");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.F15)
                Main();
            Log.SaveLogToDisk("..\\..\\..\\log.txt");
        }
    }
}
