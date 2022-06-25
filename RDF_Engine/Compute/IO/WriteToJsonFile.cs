
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
