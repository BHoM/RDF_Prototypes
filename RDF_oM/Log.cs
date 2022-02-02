using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public static class Log
    {
        public static void RecordError(string error)
        {
            Console.WriteLine($"ERROR:\n{error}");
            BH.Engine.Base.Compute.RecordError(error);
        }

        public static void RecordWarning(string warning)
        {
            Console.WriteLine($"Warning:\n{warning}");
            BH.Engine.Base.Compute.RecordWarning(warning);
        }

        public static void RecordNote(string message)
        {
            Console.WriteLine(message);
            BH.Engine.Base.Compute.RecordNote(message);
        }
    }
}
