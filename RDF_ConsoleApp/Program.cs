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
            HashSet<string> exceptions = new HashSet<string>()
            {
                "BH.oM.Base.IObject",
                "BH.oM.Base.IBHoMObject",
                "BH.oM.Base.BHoMObject",
            };

            //Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
            //    "BH.oM.Base.IBHoMObject",
            //    "BH.oM.Base.BHoMObject",
            //}, relationRecursion: 1); // this is ok

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Physical.Elements.Column",
            }, relationRecursion: 0);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
            }, relationRecursion: 0);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
            }, relationRecursion: 1);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
            }, relationRecursion: 2);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
                "BH.oM.Physical.Elements.Wall",
                "BH.oM.Architecture.Elements.Room",
                "BH.oM.Physical.Elements.Column"
            }, relationRecursion: 1);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
                "BH.oM.Physical.Elements.Wall",
                "BH.oM.Architecture.Elements.Room",
                "BH.oM.Physical.Elements.Column"
            }, relationRecursion: 2);

            Engine.RDF.Compute.WriteWebVOWLOntology(new List<string> {
                "BH.oM.Architecture.Elements.Ceiling",
                "BH.oM.Physical.Elements.Wall",
                "BH.oM.Architecture.Elements.Room",
                "BH.oM.Physical.Elements.Column"
            }, relationRecursion: 99);


            //Engine.RDF.Compute.WriteWebVOWLOntologiesPerNamespace();

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
