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
        [Description("Returns the 'namespace group' of a certain type. This is the topmost portion of the namespace to which the type belongs." +
            "Examples: a type `BH.oM.Structure.Element.Bar` is in the namespace group `BH.oM.Structure`." +
            "A type of `BH.oM.Adapters.SAP2000.Elements.SomeSapType` belongs to the namespace group `BH.oM.Adapters.SAP2000`.")]
        public static string NamespaceGroup(this Type t, int namespaceGroupDepth = 3)
        {
            // Group by namespace
            if (namespaceGroupDepth < 3)
                namespaceGroupDepth = 3; // at least group per BH.oM.Something

            string ns = t.Namespace;

            if (ns.StartsWith("BH.oM.Adapters") || ns.StartsWith("BH.oM.External"))
                namespaceGroupDepth = 4; // at least group per BH.oM.Adapters.Something

            string namespaceGroup = string.Join(".", t.Namespace.Split('.').Take(namespaceGroupDepth));

            return namespaceGroup;
        }
    }
}