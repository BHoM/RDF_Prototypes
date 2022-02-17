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
        public static List<TypeInfo> BHoMTypes(this List<Assembly> oMassemblies)
        {
            // Remove duplicate classes in the same file, e.g. `BH.oM.Base.Output` which has many generics replicas.
            List<TypeInfo> oMTypes = new List<TypeInfo>(oMassemblies
                .SelectMany(a => a.DefinedTypes)
                .Where(t => t.IsBHoMType() && !t.Name.Contains("<>c")) // removes those 'c<>' generics types that appear as duplicates
                .GroupBy(t => t.FullName.OnlyAlphabeticAndDots())
                .Select(g => g.First()));
            return oMTypes;
        }
    }
}