using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Query.Algebra;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        public static string SearchAndReplaceString(string[] text, string stringToSearchfor)
        {
            foreach (string line in text)
            {
                if (line.Contains(stringToSearchfor))
                {
                   return line.Replace(stringToSearchfor, string.Empty);
                }
            }

            return String.Empty;
        }
    }
}

