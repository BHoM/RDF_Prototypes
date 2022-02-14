using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.RDF
{
    public static class Log
    {
        private static List<string> m_allMessages = new List<string>();
        private static HashSet<string> m_reportedErrors = new HashSet<string>();
        private static HashSet<string> m_reportedWarnings = new HashSet<string>();
        private static HashSet<string> m_reportedNotes = new HashSet<string>();

        public static void RecordError(string error, bool doNotRepeat = false)
        {
            if (doNotRepeat && m_reportedErrors.Contains(error))
                return;

            Console.WriteLine($"ERROR: {error}");
            BH.Engine.Base.Compute.RecordError(error);

            if (doNotRepeat)
                m_reportedErrors.Add(error);

            m_allMessages.Add($"{DateTime.Now}\tERROR: {error}");
        }

        public static void RecordWarning(string warning, bool doNotRepeat = false)
        {
            if (doNotRepeat && m_reportedWarnings.Contains(warning))
                return;

            Console.WriteLine($"Warning: {warning}");
            BH.Engine.Base.Compute.RecordWarning(warning);

            if (doNotRepeat)
                m_reportedWarnings.Add(warning);

            m_allMessages.Add($"{DateTime.Now}\tWarning: {warning}");
        }

        public static void RecordNote(string note, bool doNotRepeat = false)
        {
            if (doNotRepeat && m_reportedNotes.Contains(note))
                return;

            Console.WriteLine(note);
            BH.Engine.Base.Compute.RecordNote(note);

            if (doNotRepeat)
                m_reportedWarnings.Add(note);

            m_allMessages.Add($"{DateTime.Now}\t{note}");
        }

        public static void SaveLogToDisk(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllLines(filePath, m_allMessages);
        }
    }

}
