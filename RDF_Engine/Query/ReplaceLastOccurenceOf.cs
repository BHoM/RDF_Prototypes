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
        public static string ReplaceLastOccurenceOf(this string text, char ch, string replaceWith)
        {
            var lastOccurrence = text.LastIndexOf(ch);
            if (lastOccurrence != -1) 
                text = text.Remove(lastOccurrence, 1).Insert(lastOccurrence, replaceWith);

            return text;
        }
    }
}
