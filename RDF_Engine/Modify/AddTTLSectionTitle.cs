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
    public static partial class Modify
    {
        public static string AddTTLSectionTitle(this string ontology, string title)
        {
            ontology += Environment.NewLine;
            ontology += Query.TTLSectionTitle(title);
            ontology += Environment.NewLine;

            return ontology;
        }
    }
}
