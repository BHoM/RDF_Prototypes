using System;
using BH.Engine.RDF;
using BH.Test.RDF;

namespace BH.oM.CodeAnalysis.ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args = null)
        {
            string exportedTTL = TTLExportTests.Room();
            Console.Write(exportedTTL);

            // Invoke all static methods in `TTLExportTests` class
            //typeof(TTLExportTests).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            // Invoke all static methods in `Tests_Diellza` class
            //typeof(Tests_Diellza).GetMethods().Where(mi => mi.IsStatic).ToList().ForEach(mi => mi.Invoke(null, null));

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
            Log.SaveLogToDisk("..\\..\\..\\log.txt");
        }
    }
}
