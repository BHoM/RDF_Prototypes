
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Writing;
using BH.Engine.Base;
using BH.oM.RDF;
using BH.oM.Base.Attributes;

namespace BH.Engine.RDF
{
    public static partial class Compute
    {
        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static List<object> ReadTTL(string TTLtext)
        {
            if (string.IsNullOrWhiteSpace(TTLtext))
                return new List<object>();

            return Convert.ToCSharpObjects(TTLtext);
        }

        [Description("Reads a TTL ontology and attempts to convert any A-Box individual into its CSharp object equivalent.")]
        public static List<object> ReadTTL(string TTLfilePath, bool active = false)
        {
            if (!active)
                return new List<object>();

            string TTLtext = File.ReadAllText(TTLfilePath);

            return ReadTTL(TTLtext);
        }
    }
}
