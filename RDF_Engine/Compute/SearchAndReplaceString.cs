using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VDS.RDF.Query.Algebra;

namespace BH.Engine.RDF
{
    public static partial class Convert
    {
        public static List<string> SearchAndReplaceString(string[] text, string stringToSearchfor)
        {
            if (text == null || text.Length == 0 && stringToSearchfor.IsNullOrEmpty()) 
                return new List<string>();

            List<string> result = new List<string>();

            foreach (string line in text)
            {
                if (line.Contains(stringToSearchfor))
                {
                  result.Add(line.Replace(stringToSearchfor, string.Empty));
                }
            }

            return result;
        }
    }
}

