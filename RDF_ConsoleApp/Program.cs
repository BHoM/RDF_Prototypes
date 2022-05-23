using System;
using BH.Engine.RDF;
using BH.oM.RDF;
using BH.Test.RDF;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args = null)
        {
            string customObjectTTL = TTLExportTests.CustomObject();
            Console.Write(customObjectTTL);

            Graph g = new Graph();
            StringParser.Parse(g, customObjectTTL);

            //Console.Write(TTLExportTests.Lists());

            //Console.Write(TTLExportTests.Room());
            //Console.Write(TTLExportTests.RoomAndColumn());

            // Invoke all static methods in `TTLExportTests` class
            //typeof(TTLExportTests).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

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
