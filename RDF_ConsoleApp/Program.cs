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
