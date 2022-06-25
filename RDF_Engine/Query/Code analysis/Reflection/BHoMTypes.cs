using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BH.Engine.RDF
{
    public static partial class Query
    {
        private static List<TypeInfo> m_cachedOmTypes = null;

        public static List<TypeInfo> BHoMTypes(this List<Assembly> oMassemblies)
        {
            if (m_cachedOmTypes != null)
                return m_cachedOmTypes;

            // Remove duplicate classes in the same file, e.g. `BH.oM.Base.Output` which has many generics replicas.
            List<TypeInfo> oMTypes = new List<TypeInfo>();
            foreach (Assembly a in oMassemblies)
            {
                if (a == null)
                    continue;

                IEnumerable<TypeInfo> typesDefinedInAssembly = null;

                try
                {
                    typesDefinedInAssembly = a.DefinedTypes?.Where(t => t.IsBHoMType());
                }
                catch (ReflectionTypeLoadException e)
                {
                    Log.RecordError($"Could not load BHoM types from assembly {a.FullName}. Error(s):\n    {string.Join("\n    ", e.LoaderExceptions.Select(le => le.Message))}");
                }

                if (typesDefinedInAssembly != null)
                    oMTypes.AddRange(typesDefinedInAssembly);
            }

            // Sort loaded type by fullname.
            oMTypes = oMTypes.GroupBy(t => t.FullName.OnlyAlphabeticAndDots())
                .Select(g => g.First()).ToList();

            m_cachedOmTypes = oMTypes;

            return oMTypes;
        }
    }
}