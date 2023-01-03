using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        public static string GetStringBetweenCharacters(string input, string sFrom, string sTo)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            String St = input;
            if (St.IndexOf(sFrom) != -1 && St.IndexOf(sTo) != -1)
            {
                int pFrom = St.IndexOf(sFrom) + sFrom.Length;
                int pTo = St.LastIndexOf(sTo);

                return St.Substring(pFrom, pTo - pFrom);
            }
            return string.Empty;

        }
    }
}
