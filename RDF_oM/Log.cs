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
        private static HashSet<string> m_reportedErrors = new HashSet<string>();
        private static HashSet<string> m_reportedWarnings = new HashSet<string>();

        public static void RecordError(string error, bool doNotRepeat = false)
        {
            if (doNotRepeat && m_reportedErrors.Contains(error))
                return;

            Console.WriteLine($"ERROR: {error}");
            BH.Engine.Base.Compute.RecordError(error);

            if (doNotRepeat)
                m_reportedErrors.Add(error);
        }

        public static void RecordWarning(string warning, bool doNotRepeat = false)
        {
            if (doNotRepeat && m_reportedWarnings.Contains(warning))
                return;

            Console.WriteLine($"Warning: {warning}");
            BH.Engine.Base.Compute.RecordWarning(warning);

            if (doNotRepeat)
                m_reportedWarnings.Add(warning);
        }

        public static void RecordNote(string message)
        {
            Console.WriteLine(message);
            BH.Engine.Base.Compute.RecordNote(message);
        }
    }
}
