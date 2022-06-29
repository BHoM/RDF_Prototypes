using System;
using System.Linq;
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
