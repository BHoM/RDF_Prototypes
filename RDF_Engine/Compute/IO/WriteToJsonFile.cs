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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;


namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        public static void WriteToJsonFile(this IGraph graph, string filename = "RDF_Prototypes_test.rdf", string directory = @"C:\temp\RDF_Prototypes_test")
        {
            RdfJsonWriter rdfJsonWriter = new RdfJsonWriter();
            System.IO.StringWriter sw = new System.IO.StringWriter();

            //Call the Save() method to write to the StringWriter
            rdfJsonWriter.Save(graph, sw);

            //We can now retrieve the written RDF by using the ToString() method of the StringWriter
            String data = sw.ToString();
            Console.WriteLine(data);

            System.IO.Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, filename), data);
        }

        public static void WriteToJsonFile(this string text, string filename = "RDF_Prototypes_test.rdf", string directory = @"C:\temp\RDF_Prototypes_test")
        {
            Directory.CreateDirectory(directory);

            string filepath = Path.Combine(directory, filename);

            try
            {
                File.WriteAllText(filepath, text);
            }
            catch (Exception e)
            {
                Log.RecordError($"Could not write file `{filepath}`. Exception:\n{e.ToString()}");
            }
        }
    }
}
