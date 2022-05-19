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
    public static partial class Create
    {
        public static string TTLSectionTitle(this string title)
        {
            return $"\n\n\n#################################################################\n#    {title}\n#################################################################\n\n";
        }
    }
}
