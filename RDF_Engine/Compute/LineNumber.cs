using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.RDF;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        private static Dictionary<string, List<string>> m_cachedFileLines = new Dictionary<string, List<string>>();
        private static Dictionary<PropertyInfo, int> m_cachedPinfoFileline = new Dictionary<PropertyInfo, int>();

        [Description("Looks for the line number of a property in its `.cs` file by reading the file. Returns -1 if not found. 0 indicates the first line.")]
        public static int LineNumber(PropertyInfo pi, LocalRepositorySettings settings)
        {
            int index = -1;

            if (settings.ReadCacheFiles && ReadCache_PInfoFileline(settings) && m_cachedPinfoFileline.TryGetValue(pi, out index))
                return index;

            string declaringTypeFilePath = pi.DeclaringType.FilePathFromLocalRepository(settings, false);

            List<string> lines = null;

            if (!string.IsNullOrWhiteSpace(declaringTypeFilePath) && !m_cachedFileLines.TryGetValue(declaringTypeFilePath, out lines))
            {
                lines = File.ReadAllLines(declaringTypeFilePath).ToList();
                m_cachedFileLines[declaringTypeFilePath] = lines;
            }

            // Get the index from the read lines of the file.
            index = lines?.FindIndex(l =>
            l.Contains(pi.Name) &&
            l.ToLower().Contains(pi.PropertyType.GetCodeName().ToLower()) &&
            //l.Contains("public") &&
            l.Contains("{"))
                ?? -1;

            if (index == -1)
                Log.RecordWarning($"Could not find Line Number of property {pi.FullNameValidChars()}");
            else
                m_cachedPinfoFileline[pi] = index;

            return index;
        }

        private static bool ReadCache_PInfoFileline(LocalRepositorySettings settings)
        {
            if (m_cachedPinfoFileline != null && m_cachedPinfoFileline.Any())
                return true;

            Dictionary<PropertyInfo, int> read = null;

            try
            {
                string path = Path.Combine(settings.CacheRootPath, settings.CacheFileName_PropertyInfoFileLines);
                read = JsonConvert.DeserializeObject<Dictionary<PropertyInfo, int>>(path);
            }
            catch
            {
                return false;
            }

            if (read == null)
                return false;

            m_cachedPinfoFileline = read;

            return true;
        }

        public static bool WriteCache_PInfoFileline(LocalRepositorySettings settings)
        {
            if (m_cachedPinfoFileline == null && !m_cachedPinfoFileline.Any())
                return false;

            string path = Path.Combine(settings.CacheRootPath, settings.CacheFileName_PropertyInfoFileLines);
            File.WriteAllText(path, JsonConvert.SerializeObject(m_cachedPinfoFileline));

            return true;
        }

        public static int LineNumber(MemberInfo mi, LocalRepositorySettings settings)
        {
            string declaringTypeFilePath = mi.DeclaringType.FilePathFromLocalRepository(settings);

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
