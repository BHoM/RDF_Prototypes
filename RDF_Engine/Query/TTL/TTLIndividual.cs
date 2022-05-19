using BH.oM.Base;
using BH.oM.RDF;
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
        public static string TTLIndividual(string projectUri, string individualUri, string className, string en_label, Dictionary<string, string> properties = null)
        {
            string composed = $"### {projectUri}/{individualUri}";
            composed += $"\n{className} rdf:type owl:Class;";
            if (properties?.Any() ?? false)
                foreach (var kv in properties)
                {
                    composed += string.Join($"\n\t\t:", kv.Key) + $@"""{kv.Value}""; ";
                }

            composed = composed.ReplaceLastOccurenceOf(';', ".");

            return composed;
        }


    }
}
