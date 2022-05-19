using BH.oM.Analytical.Results;
using BH.oM.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.RDF;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.oM.Architecture.Elements;
using BH.oM.Physical.Elements;
using BH.oM.RDF;
using Newtonsoft.Json.Linq;
using BH.Engine.Base;
using log = BH.oM.RDF.Log;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args = null)
        {
            Column randomColumn = BH.Engine.RDF.Testing.Create.RandomObject<Column>();
            CSharpGraph cSharpGraph = Engine.RDF.Compute.CSharpGraph(randomColumn, new OntologySettings());
            LocalRepositorySettings diellzasettings = new LocalRepositorySettings()
            {
                RepositoryRootPath = @"C:\Users\diels\source"
            };
            string TTLGraph = Engine.RDF.Compute.TTLGraph(randomColumn, new OntologySettings(), diellzasettings);

            CustomObject customObject = BH.Engine.Base.Create.CustomObject(new Dictionary<string, object>() { { "firstProperty", 10 }, { "secondProperty", 20 } });
            CSharpGraph cSharpGraph_customObj = Engine.RDF.Compute.CSharpGraph(customObject, new OntologySettings());


            // Invoke all static methods in `Tests_Alessio` class
            //typeof(Tests_Alessio).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            // Invoke all static methods in `Tests_Diellza` class
            //typeof(Tests_Diellza).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
            log.SaveLogToDisk("..\\..\\..\\log.txt");
        }
    }
}
