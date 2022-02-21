using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log = BH.oM.RDF.Log;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        private static Dictionary<string, List<string>> m_cachedFileLines = new Dictionary<string, List<string>>();

        [Description("Looks for the line number of a property in its `.cs` file by reading the file. Returns -1 if not found. 0 indicates the first line.")]
        public static int LineNumber(PropertyInfo pi, string repositoryRoot, string cacheRootDirectory = null)
        {
            string declaringTypeFilePath = pi.DeclaringType.FilePathFromLocalRepository(repositoryRoot);

            List<string> lines = null;

            if (!string.IsNullOrWhiteSpace(declaringTypeFilePath) && !m_cachedFileLines.TryGetValue(declaringTypeFilePath, out lines))
            {
                lines = File.ReadAllLines(declaringTypeFilePath).ToList();
                m_cachedFileLines[declaringTypeFilePath] = lines;
            }

            int index = lines?.FindIndex(l =>
            l.Contains(pi.Name) &&
            l.ToLower().Contains(pi.PropertyType.GetCodeName().ToLower()) &&
            //l.Contains("public") &&
            l.Contains("{"))
                ?? -1;

            if (index == -1)
                log.RecordWarning($"Could not find Line Number of property {pi.FullNameValidChars()}");

            return index;
        }

        public static int LineNumber(MemberInfo mi, string repositoryRoot, string cacheRootDirectory = null)
        {
            string declaringTypeFilePath = mi.DeclaringType.FilePathFromLocalRepository(repositoryRoot);

            List<string> lines = null;

            if (!m_cachedFileLines.TryGetValue(declaringTypeFilePath, out lines))
            {
                lines = File.ReadAllLines(declaringTypeFilePath).ToList();
                m_cachedFileLines[declaringTypeFilePath] = lines;
            }

            int index = lines.FindIndex(l => l.Contains(mi.Name));

            return index;
        }
    }
}
